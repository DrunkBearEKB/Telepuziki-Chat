using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using NetworkLibrary.Common.Package;
using NetworkLibrary.Common.Package.ExchangingPackages;

namespace NetworkLibrary.Server
{
    public class ServerObject
    {
        private readonly TcpListener listener;
        private readonly Dictionary<string, ConnectedClient> dictionaryConnectedClients;
        private readonly int port = 9090;
        private readonly PackageCreator packageCreator;

        private IHistory history;

        private Timer timerOnlineChecking;
        private const int amountSecondsBetweenOnlineChecking = 5;

        public ServerObject()
        {
            this.listener = new TcpListener(IPAddress.Any, this.port);
            this.dictionaryConnectedClients = new Dictionary<string, ConnectedClient>();
            this.packageCreator = new PackageCreator();

            this.LoadMessageHistory();
        }

        public async Task Start()
        {
            this.listener.Start();
            Console.WriteLine($"Server started on port: {port}.");

            this.timerOnlineChecking = new Timer(amountSecondsBetweenOnlineChecking * 1000);
            this.timerOnlineChecking.Elapsed += this.OnTimerOnlineCheckingTick;
            this.timerOnlineChecking.AutoReset = true;
            this.timerOnlineChecking.Enabled = true;

            while (true)
            {
                TcpClient client = await this.listener.AcceptTcpClientAsync();

                int amountBytesReaded;
                while (!this.packageCreator.CanGetPackage)
                {
                    byte[] bytes = new byte[1024];
                    amountBytesReaded = client.GetStream().Read(bytes, 0, bytes.Length);
                    if (amountBytesReaded != 0)
                    {
                        packageCreator.Add(bytes, amountBytesReaded);
                    }
                }
                IPackage package = packageCreator.GetPackage();
                
                if (!this.dictionaryConnectedClients.ContainsKey(package.IdAuthor))
                {
                    ConnectedClient connectedClient = (ConnectedClient) client;
                    connectedClient.Online = true;
                    
                    this.dictionaryConnectedClients.Add(package.IdAuthor, connectedClient);
                }
                else
                {
                    // TODO Логика работы в ситуации, когда к серверу пробует подключиться уже подключенный клиент
                }
            }
        }
        
        private void OnTimerOnlineCheckingTick(object source, ElapsedEventArgs e)
        {
            IPackage package;
            
            foreach (string id in this.dictionaryConnectedClients.Keys)
            {
                if (!this.dictionaryConnectedClients[id].Online)
                {
                    package = new DisconnectPackage("");

                    if (this.dictionaryConnectedClients[id].GetStream().CanWrite)
                    {
                        this.dictionaryConnectedClients[id].GetStream().Write(package);
                    }
                    else
                    {
                        // TODO Логика работы в ситуации, когда нет возможности отправить DisconnectPackage клиенту
                    }
                    
                    this.dictionaryConnectedClients.Remove(id);
                }
                else
                {
                    this.dictionaryConnectedClients[id].Online = false;
                    package = new OnlinePackage("");
                   
                    if (this.dictionaryConnectedClients[id].GetStream().CanWrite)
                    {
                        this.dictionaryConnectedClients[id].GetStream().Write(package);
                    }
                    else
                    {
                        // TODO Логика работы в ситуации, когда нет возможности отправить OnlinePackage клиенту
                    }
                }
            }
        }

        private void LoadMessageHistory()
        {
            // TODO Логика работы при загрузке истории
        }
    }
}