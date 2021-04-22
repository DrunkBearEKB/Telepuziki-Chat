using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly string ipServer = "127.0.0.1";  // 192.168.88.71  127.0.0.1
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
                    // TODO Логика работы в ситуации когда произошло исключение SocketException
                }
                catch (IOException)
                {
                    // TODO Логика работы в ситуации когда произошло исключение IOException
                }
            }
        }

        private async Task HandlePackage()
        {
            IPackage package = this.packageCreator.GetPackage();

            switch (package)
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
                    this.OnOnlineChecking?.Invoke();
                    await this.SendOnlineToServer();
                    break;
                        
                case DisconnectPackage:
                    this.OnDisconnectFromServerForced?.Invoke();
                    await this.ConnectToServer();
                    break;
                
                case HistoryRequestPackage historyRequestPackage:
                    // TODO Логика работы в ситуации когда пришёл запрос от сервера на получение истории сообщений
                    break;
                
                case HistoryAnswerPackage historyAnswerPackage:
                    // TODO Логика работы в ситуации когда пришёл ответ от сервера на получение истории сообщений
                    break;
            }
        }

        public delegate void TextMessageHandler(TextMessage message);
        public event TextMessageHandler OnTextMessageReceive;
        
        public delegate void VoiceMessageHandler(VoiceMessage message);
        public event VoiceMessageHandler OnVoiceMessageReceive;
        
        public delegate void FileMessageHandler(FileMessage message);
        public event FileMessageHandler OnFileMessageReceive;
        
        public delegate void OnlineCheckingHandler();
        public event OnlineCheckingHandler OnOnlineChecking;
        
        public delegate void HistoryRequestHandler();
        public event HistoryRequestHandler OnHistoryRequest;
        
        public delegate void HistoryAnswerHandler(IEnumerable<IMessage> listMessages);
        public event HistoryAnswerHandler OnHistoryReceive;
        
        public delegate void DisconnectFromServerForcedHandler();
        public event DisconnectFromServerForcedHandler OnDisconnectFromServerForced;
    }
}