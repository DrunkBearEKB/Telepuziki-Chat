using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks
    ;
using Network.Extensions;
using Network.Message;
using Network.Message.ExchangingMessages;
using Network.Package;
using Network.Package.ExchangingPackages;

namespace Client.Network
{
    public class ClientObject
    {
        private TcpClient tcpClient;
        private readonly string ipServer = "192.168.88.71";  // 169.254.222.91  192.168.88.71
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
            await this.tcpClient.ConnectAsync(this.ipServer, 9090);
            this.stream = this.tcpClient.GetStream();
            await this.SendOnlineToServer();
            
            await this.StartReceivingPackages();
        }
        
        public async Task SendMessage(string idReceiver, string data)
        {
            IPackage package = new TextPackage(idReceiver, this.Id, DateTime.Now, data);
            await this.stream.WriteAsync(package);
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
                int amountBytesReaded = await this.stream.ReadAsync(bytes, 0, bytes.Length);
                if (amountBytesReaded != 0)
                {
                    this.packageCreator.Add(bytes, amountBytesReaded);
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
                    await this.SendOnlineToServer();
                    break;
                        
                case DisconnectPackage:
                    await this.tcpClient.ConnectAsync(this.ipServer, 9090);
                    this.stream = this.tcpClient.GetStream();
                    await this.SendOnlineToServer();
                            
                    // TODO Логика работы в ситуации когда произошло отключение от сервера
                            
                    break;
            }
        }

        public delegate void TextPackageReceiveHandler(TextMessage message);
        public event TextPackageReceiveHandler OnTextMessageReceive;
        
        public delegate void VoicePackageReceiveHandler(VoiceMessage message);
        public event VoicePackageReceiveHandler OnVoiceMessageReceive;
        
        public delegate void FilePackageReceiveHandler(FileMessage message);
        public event FilePackageReceiveHandler OnFileMessageReceive;
        
        public delegate void HistoryReceiveHandler(IEnumerable<IMessage> listMessages);
        public event HistoryReceiveHandler OnHistoryMessageReceive;
    }
}