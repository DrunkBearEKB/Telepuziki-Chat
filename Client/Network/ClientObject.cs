using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using DataBase;
using DataBase.Chat;
using DataBase.User;
using Microsoft.VisualBasic.ApplicationServices;
using Network.Extensions;
using Network.Message;
using Network.Message.ExchangingMessages;
using Network.Package;
using Network.Package.ExchangingPackages;
using NUnit.Framework;
using User = DataBase.User.User;

namespace Client.Network
{
    public class ClientObject
    {
        private TcpClient tcpClient;
        private readonly string ipServer = "192.168.1.3";  // 192.168.88.71  127.0.0.1  192.168.43.12
        private readonly int portServer = 9090;
        private System.Net.Sockets.NetworkStream stream;
        
        private readonly PackageCreator packageCreator;

        public bool AutoReconnect = true;
        public bool IsConnected { get; private set; }
        public string Id { get; }
        private User user;
        public bool IsHistoryReceived { get; private set; }
        private WrappedFirebase dataBase;

        public ClientObject(string id)
        {
            Id = id;
            packageCreator = new PackageCreator();
            
            dataBase = new WrappedFirebase();
            var response = dataBase.GetUser(this.Id);
            if (response == null)
            {
                user = new User(id, "123", id);
                dataBase.SetUser(user);
                
            }
            else
            {
                this.user = response;
            }

            if (user.Chats.Count == 0)
            {
                var chat = new Chat($"{id} {id}", "", new List<User>() {
                    user, user
                }, new List<TextMessage>() {new TextMessage(Id, Id, DateTime.Now, 
                    "Привет, тут ты можешь писать сам себе)")});
                dataBase.SetChat(chat);
                this.user.Chats.Add(chat);
            }

        }
        
        public async void Start()
        {
            await this.ConnectToServer();
            await this.StartReceivingPackages();
        }

        public async Task SendText(string idReceiver, string data)
        {
            await this.SendPackage(new TextPackage(idReceiver, this.Id, DateTime.Now, data));
        }
        
        public async Task SendFile(string idReceiver, byte[] data)
        {
            await this.SendPackage(new FilePackage(idReceiver, this.Id, DateTime.Now, data));
        }
        
        public async Task SendVoice(string idReceiver, byte[] data)
        {
            await this.SendPackage(new VoicePackage(idReceiver, this.Id, DateTime.Now, data));
        }
        
        public async Task SendSearchRequest(string idRequest)
        {
            await this.SendPackage(new UsersListRequestPackage("", this.Id, idRequest));
        }

        public Dictionary<string, List<TextMessage>> RequestHistory()
        {
            Dictionary<string, List<TextMessage>> result = new Dictionary<string, List<TextMessage>>();
            
            foreach (var chat in this.user.Chats)
            {
                if (chat.Id != $"{this.Id} {this.Id}")
                    result.Add(chat.Members.Select(u => u.Id).First(id => id != this.Id), 
                        chat.Messages ?? new List<TextMessage>());
                else
                {
                    result.Add(this.Id, this.dataBase.GetChat($"{this.Id} {this.Id}").Messages ?? new List<TextMessage>());
                }
            }

            return result;
        }

        private async Task SendPackage(IPackage package)
        {
            try
            {
                Console.WriteLine(package.Type);
                await this.stream.WriteAsync(package);
            }
            catch (IOException)
            {
                await this.HandleDisconnect();
            }
        }

        private async Task ConnectToServer()
        {
            while (true)
            {
                try
                {
                    this.tcpClient = new TcpClient();
                    await this.tcpClient.ConnectAsync(this.ipServer, this.portServer);
                    
                    this.stream = this.tcpClient.GetStream();
                    await this.SendOnlineToServer();
                    this.IsConnected = true;
                    break;
                }
                catch
                {
                    // ignored
                }
            }
        }
        
        private async Task SendOnlineToServer()
        {
            IPackage package = new OnlinePackage("", this.Id);
            await this.stream.WriteAsync(package);
        }

        private async Task StartReceivingPackages()
        {
            while (true)
            {
                await this.ReceivePackage();
                if (this.IsConnected)
                {
                    await this.HandleReceivedPackage();
                }
                else
                {
                    break;
                }
            }
            
            await this.HandleDisconnect();
        }

        private async Task ReceivePackage()
        {
            while (!this.packageCreator.CanGetPackage)
            {
                byte[] bytes = new byte[1024];

                try
                {
                    int amountBytesRead = await this.stream.ReadAsync(bytes, 0, bytes.Length);
                    if (amountBytesRead != 0)
                    {
                        this.packageCreator.Add(bytes, amountBytesRead);
                    }
                }
                catch
                {
                    break;
                }
            }

            if (!this.packageCreator.CanGetPackage)
            {
                await this.HandleDisconnect();
            }
        }

        private async Task HandleReceivedPackage()
        {
            IPackage package = this.packageCreator.GetPackage();

            switch (package)
            {
                case TextPackage textPackage:
                    this.OnTextMessageReceive?.Invoke(
                        package.IdAuthor, new TextMessage(this.Id, textPackage.IdAuthor, textPackage.Time, textPackage.Content));
                    break;
                        
                case VoicePackage voicePackage:
                    this.OnVoiceMessageReceive?.Invoke(
                        package.IdAuthor, new VoiceMessage(this.Id, voicePackage.IdAuthor, voicePackage.Time, voicePackage.Content));
                    break;
                        
                case FilePackage filePackage:
                    this.OnFileMessageReceive?.Invoke(
                        package.IdAuthor, new FileMessage(this.Id, filePackage.IdAuthor, filePackage.Time, filePackage.Content));
                    break;
                        
                case OnlinePackage:
                    this.OnOnlineChecking?.Invoke();
                    await this.SendOnlineToServer();
                    break;
                        
                case DisconnectPackage:
                    await this.HandleDisconnect();
                    break;
                
                case HistoryRequestPackage historyRequestPackage:
                    // TODO Логика работы в ситуации когда пришёл запрос от сервера на получение истории сообщений
                    break;
                
                case HistoryAnswerPackage historyAnswerPackage:
                    // TODO Логика работы в ситуации когда пришёл ответ от сервера на получение истории сообщений
                    break;
                
                case UsersListAnswerPackage usersListAnswerPackage:
                    this.OnUsersListAnswerReceive?.Invoke(usersListAnswerPackage.Users);
                    break;
            }
        }

        public async Task Disconnect()
        {
            await this.SendPackage(new DisconnectPackage("", this.Id));
        }

        private async Task HandleDisconnect()
        {
            if (this.IsConnected)
            {
                this.IsConnected = false;
                this.OnDisconnectFromServer?.Invoke();
                if (this.AutoReconnect)    
                {   
                    await this.ConnectToServer();
                }
            }
        }

        public delegate void TextMessageHandler(string idSender, TextMessage message);
        public event TextMessageHandler OnTextMessageReceive;
        
        public delegate void VoiceMessageHandler(string idSender,VoiceMessage message);
        public event VoiceMessageHandler OnVoiceMessageReceive;
        
        public delegate void FileMessageHandler(string idSender,FileMessage message);
        public event FileMessageHandler OnFileMessageReceive;
        
        public delegate void OnlineCheckingHandler();
        public event OnlineCheckingHandler OnOnlineChecking;
        
        public delegate void HistoryRequestHandler();
        public event HistoryRequestHandler OnHistoryRequest;
        
        public delegate void HistoryAnswerHandler(string id, IEnumerable<TextMessage> listMessages);
        public event HistoryAnswerHandler OnHistoryReceive;
        
        public delegate void DisconnectFromServerForcedHandler();
        public event DisconnectFromServerForcedHandler OnDisconnectFromServer;
        
        public delegate void UsersListAnswerHandler(List<string> users);
        public event UsersListAnswerHandler OnUsersListAnswerReceive;
    }
}