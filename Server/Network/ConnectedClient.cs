using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using DataBase.Chat;
using DataBase.User;
using Network.Extensions;
using Network.Message;
using Network.Message.ExchangingMessages;
using Network.Package;
using Network.Package.ExchangingPackages;

namespace Server.Network
{
    public class ConnectedClient : IDisposable
    {
        public string Id { get; private set; }
        private bool online;
        
        private readonly NetworkStream stream;
        private readonly ServerObject server;

        private PackageCreator packageCreator;

        public ConnectedClient(NetworkStream stream, ServerObject server)
        {
            this.online = true;
            this.stream = stream;
            this.server = server;
            
            this.packageCreator = new PackageCreator();
        }
        
        public ConnectedClient(string id, NetworkStream stream, ServerObject server) : this(stream, server)
        {
            this.Id = id;
        }
        
        public async Task SendPackage(IPackage package)
        {
            try
            {
                await this.stream.WriteAsync(package);
            }
            catch
            {
                await this.server.Disconnect(this.Id);
                this.OnDisconnected?.Invoke();
            }
        }

        public async void StartListen()
        {
            while (true)
            {
                try
                {
                    IPackage package = await this.ReceivePackage();
                    await this.HandleReceivedPackage(package);
                }
                catch
                {
                    if (this.stream.CanWrite)
                    {
                        await this.server.Disconnect(this.Id);
                        await this.Disconnect();
                    }
                    
                    // TODO Логика работы,в ситуации, когда происходит ошибка при попытке получения данных от клиента
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task<IPackage> ReceivePackage()
        {
            byte[] bytesBuffer = new byte[1024];
            
            while (!this.packageCreator.CanGetPackage)
            {
                try
                {
                    int amount = await this.stream.ReadAsync(bytesBuffer);
                    this.packageCreator.Add(bytesBuffer, amount);
                }
                catch (SocketException)
                {
                    // TODO Логика работы в ситуации когда произошла ошибка связи с клиентом
                }
            }

            return this.packageCreator.GetPackage();
        }

        private async Task HandleReceivedPackage(IPackage package)
        {
            this.online = true;
            
            if (package.IdReceiver != "")
            {
                await this.server.SendPackage(package);

                if (package.Type == PackageType.Text || package.Type == PackageType.Voice ||
                    package.Type == PackageType.File)
                {
                    IMessage message = package switch
                    {
                        TextPackage textPackage => new TextMessage(this.Id, textPackage.IdAuthor, textPackage.Time,
                            textPackage.Content),
                        VoicePackage voicePackage => new VoiceMessage(this.Id, voicePackage.IdAuthor, voicePackage.Time,
                            voicePackage.Content),
                        FilePackage filePackage => new FileMessage(this.Id, filePackage.IdAuthor, filePackage.Time,
                            filePackage.Content)
                    };
                    // Тут нужен текст сообщения еще
                    // Лучше в Imessage еще хранить chatId, и текст
                    var chatId = string.Compare(this.Id, message.IdAuthor) == -1 ?
                        $"{Id} {message.IdAuthor}" :
                        $"{message.IdAuthor} {Id}";
                    if (!this.server.dataBase.GetUser(package.IdAuthor).Chats.Select(chat => chat.Id).Contains(chatId))
                    {
                        this.server.dataBase.SetChat(new Chat(chatId, "", new List<User>()
                        {
                            this.server.dataBase.GetUser(package.IdAuthor),
                            this.server.dataBase.GetUser(package.IdReceiver)
                        }));
                    }
                    this.server.dataBase.SetMessage(message, chatId);
                    // Console.WriteLine(this.server.dataBase.GetChat(chatId).Messages.Count);

                    //this.server.ServerDataBase.AddMessage(message);
                }
                
                this.OnGetPackage?.Invoke(package);
            }
            else
            {
                switch (package)
                {
                    case DisconnectPackage:
                        await this.Disconnect();
                        await this.server.Disconnect(this.Id);
                        break;
                
                    /*case HistoryRequestPackage historyRequestPackage:
                        List<IMessage> messages = this.server.ServerDataBase.GetMessages(
                            this.Id, historyRequestPackage.IdRequest, historyRequestPackage.TimeUntil);
                        HistoryAnswerPackage packageAnswer1 = new HistoryAnswerPackage(this.Id,"", 
                            messages.Select(message =>
                            {
                                Tuple<MessageType, string> result = message switch
                                {
                                    TextMessage textMessage => new Tuple<MessageType, string>(message.Type,
                                        textMessage.Content),
                                    VoiceMessage => new Tuple<MessageType, string>(message.Type, "voice message"),
                                    FileMessage => new Tuple<MessageType, string>(message.Type, "file")
                                };
                                return result;
                            }).ToList());
                        await this.server.SendPackage(packageAnswer1);
                        break;*/
                    
                    case UsersListRequestPackage usersListRequestPackage:
                        /*List<string> users = this.server.ServerDataBase
                            .GetUsersWithSimilarId(usersListRequestPackage.IdRequest);*/
                        /*List<string> users = new List<string>()
                        {
                            "Artem", "Grisha", "Julia", "Vova", "temp1", "temp2", "temp3", "temp4", "temp5"
                        };*/
                        List<string> users = this.server.dataBase.GetAllUsers().Select(user => user.Id).ToList();
                        UsersListAnswerPackage packageAnswer2 = new UsersListAnswerPackage(this.Id, "", users);
                        await this.server.SendPackage(packageAnswer2);
                        break;
                }
            }
        }

        public async Task CheckOnline()
        {
            try
            {
                if (!this.online)
                {
                    await this.stream.WriteAsync(new DisconnectPackage(this.Id, ""));
                    this.OnDisconnected?.Invoke();
                }
                else
                {
                    await this.stream.WriteAsync(new OnlinePackage(this.Id, ""));
                    this.online = false;
                }
            }
            catch
            {
                // TODO Логика работы в ситуации когда произошла ошибка связи с клиентом
            }
        }

        public async Task Disconnect()
        {
            await this.SendPackage(new DisconnectPackage(this.Id, ""));
            this.stream.Close();
            this.OnDisconnected?.Invoke();
        }
            
        public delegate void GetPackageHandler(IPackage package);
        public event GetPackageHandler OnGetPackage;
        
        public delegate void ClientDisconnectedHandler();
        public event ClientDisconnectedHandler OnDisconnected;

        public void Dispose()
        {
            this.stream?.Dispose();
        }
    }
}