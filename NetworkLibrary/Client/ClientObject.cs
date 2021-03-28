using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetworkLibrary.Common.Message;
using NetworkLibrary.Common.Message.ExchangingMessages;
using NetworkLibrary.Common.Package;
using NetworkLibrary.Common.Package.ExchangingPackages;

namespace NetworkLibrary.Client
{
    public class ClientObject
    {
        private TcpClient tcpClient;
        private readonly string ipServer = "192.168.88.71";
        private NetworkStream stream;

        private Thread threadReceivePackages;
        private readonly PackageCreator packageCreator;

        public string Id;
        
        public delegate void TextPackageReceiveHandler(TextMessage message);
        public event TextPackageReceiveHandler OnTextMessageReceive;
        
        public delegate void VoicePackageReceiveHandler(VoiceMessage message);
        public event VoicePackageReceiveHandler OnVoiceMessageReceive;
        
        public delegate void FilePackageReceiveHandler(FileMessage message);
        public event FilePackageReceiveHandler OnFileMessageReceive;
        
        public delegate void HistoryReceiveHandler(IEnumerable<IMessage> listMessages);
        public event HistoryReceiveHandler OnHistoryMessageReceive;
        
        public ClientObject(string id)
        {
            this.Id = id;
            this.tcpClient = new TcpClient();
            this.packageCreator = new PackageCreator();
        }
        
        public void Start()
        {
            this.tcpClient.Connect(this.ipServer, 9090);
            this.stream = this.tcpClient.GetStream();
            
            this.SendOnlineToServer();

            this.threadReceivePackages = new Thread(this.ReceivePackages);
            this.threadReceivePackages.Start();
        }
        
        public void SendMessage(string idReceiver, string data)
        {
            IPackage package = new TextPackage(idReceiver, this.Id, DateTime.Now, data);
            this.stream.Write(package);
        }

        private void SendOnlineToServer()
        {
            OnlinePackage package = new OnlinePackage(this.Id);
            this.stream.Write(package);
        }

        private void ReceivePackages()
        {
            int amountBytesReaded;
            while (true)
            {
                try
                {
                    while (!this.packageCreator.CanGetPackage)
                    {
                        byte[] bytes = new byte[1024];
                        amountBytesReaded = this.stream.Read(bytes, 0, bytes.Length);
                        if (amountBytesReaded != 0)
                        {
                            this.packageCreator.Add(bytes, amountBytesReaded);
                        }
                    }

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
                            this.stream.Write(new OnlinePackage(this.Id));
                            break;
                        
                        case DisconnectPackage:
                            this.tcpClient.Connect(this.ipServer, 9090);
                            this.stream = this.tcpClient.GetStream();
                            this.stream.Write(new OnlinePackage(this.Id));
                            
                            // TODO Логика работы в ситуации когда произошло отключение от сервера
                            
                            break;
                    }
                }
                catch (Exception)
                {
                    // TODO Логика работы в ситуации, когда при получении данных клинетом произошла некая ошибка
                }
            }
        }

        public void Dispose()
        {
            try
            {
                this.threadReceivePackages.Abort();
                this.threadReceivePackages.Join();

                this.tcpClient.Close();
                this.tcpClient.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}