using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

using Network.Extensions;
using Network.Message;
using Network.Message.ExchangingMessages;
using Network.Package;
using Network.Package.ExchangingPackages;

namespace Client.Network
{
    public class ClientObject
    {
        private readonly TcpClient tcpClient;
        private readonly string ipServer = "192.168.88.71";  // 192.168.88.71  169.254.222.91
        private readonly int portServer = 9090;
        private NetworkStream stream;
        
        private readonly PackageCreator packageCreator;

        public string Id { get; private set; }

        public ClientObject(string id)
        {
            this.Id = id;
            this.tcpClient = new TcpClient();
            this.packageCreator = new PackageCreator();
        }
        
        public async Task Start()
        {
            await this.ConnectToServer();
            await this.StartReceivingPackages();
        }

        public async Task SendText(string idReceiver, string data)
        {
            IPackage package = new TextPackage(idReceiver, this.Id, DateTime.Now, data);
            await this.stream.WriteAsync(package);
        }
        
        public async Task SendFile(string idReceiver, byte[] data)
        {
            IPackage package = new FilePackage(idReceiver, this.Id, DateTime.Now, data);
            await this.stream.WriteAsync(package);
        }
        
        public async Task SendVoice(string idReceiver, byte[] data)
        {
            IPackage package = new VoicePackage(idReceiver, this.Id, DateTime.Now, data);
            await this.stream.WriteAsync(package);
        }

        private async Task ConnectToServer()
        {
            await this.tcpClient.ConnectAsync(this.ipServer, this.portServer);
            this.stream = this.tcpClient.GetStream();
            await this.SendOnlineToServer();
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
                await this.HandlePackage();
            }
            // ReSharper disable once FunctionNeverReturns
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
                catch (SocketException)
                {
                    await this.ConnectToServer();
                    this.OnDisconnectFromServer?.Invoke();

                    // TODO Логика работы в ситуации когда произошло отключение от сервера 
                }
            }
        }

        private async Task HandlePackage()
        {
            switch (this.packageCreator.GetPackage())
            {
                case TextPackage textPackage:
                    this.OnTextMessageReceive?.Invoke(
                        new TextMessage(textPackage.IdAuthor, textPackage.Time, textPackage.Content));
                    break;
                        
                case VoicePackage voicePackage:
                    this.OnVoiceMessageReceive?.Invoke(
                        new VoiceMessage(voicePackage.IdAuthor, voicePackage.Time, voicePackage.Content));
                    break;
                        
                case FilePackage filePackage:
                    this.OnFileMessageReceive?.Invoke(
                        new FileMessage(filePackage.IdAuthor, filePackage.Time, filePackage.Content));
                    break;
                        
                case OnlinePackage:
                    this.OnOnlinePackageReceive?.Invoke();
                    await this.SendOnlineToServer();
                    break;
                        
                case DisconnectPackage:
                    this.OnDisconnectPackage?.Invoke();
                    await this.ConnectToServer();
                    // TODO Логика работы в ситуации когда произошло отключение от сервера (принудительное)
                    break;
                
                case HistoryRequestPackage historyRequestPackage:
                    // TODO Логика работы в ситуации когда пришёл запрос от сервера на получение истории сообщений
                    break;
                
                case HistoryAnswerPackage historyAnswerPackage:
                    // TODO Логика работы в ситуации когда пришёл ответ от сервера на получение истории сообщений
                    break;
            }
        }

        public delegate void TextPackageReceiveHandler(TextMessage message);
        public event TextPackageReceiveHandler OnTextMessageReceive;
        
        public delegate void VoicePackageReceiveHandler(VoiceMessage message);
        public event VoicePackageReceiveHandler OnVoiceMessageReceive;
        
        public delegate void FilePackageReceiveHandler(FileMessage message);
        public event FilePackageReceiveHandler OnFileMessageReceive;
        
        public delegate void OnlinePackageHandler();
        public event OnlinePackageHandler OnOnlinePackageReceive;
        
        public delegate void DisconnectPackageHandler();
        public event DisconnectPackageHandler OnDisconnectPackage;
        
        public delegate void HistoryRequestPackageHandler(IEnumerable<IMessage> listMessages);
        public event HistoryRequestPackageHandler OnHistoryRequestPackageReceive;
        
        public delegate void HistoryAnswerPackageHandler(IEnumerable<IMessage> listMessages);
        public event HistoryAnswerPackageHandler OnHistoryAnswerPackageReceive;
        
        public delegate void DisconnectFromServerHandler();
        public event DisconnectFromServerHandler OnDisconnectFromServer;
    }
}