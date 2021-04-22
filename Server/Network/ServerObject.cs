using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

using Network.Package;

using Server.History;

namespace Server.Network
{
    public class ServerObject
    {
        private readonly TcpListener listener;
        private readonly Dictionary<string, ConnectedClient> dictionaryConnectedClients;
        private readonly int port = 9090;
        private readonly PackageCreator packageCreator;

        private IHistory history;

        private Timer timerOnlineChecking;
        private const int AmountSecondsBetweenOnlineChecking = 5;

        public ServerObject()
        {
            this.listener = new TcpListener(IPAddress.Any, port);
            this.dictionaryConnectedClients = new Dictionary<string, ConnectedClient>();
            this.packageCreator = new PackageCreator();
        }

        public async Task Start()
        {
            await this.LoadMessageHistory();
            
            this.listener.Start();

            this.StartTimers();

            this.OnServerStarted?.Invoke();

            await this.StartListening();
        }

        private void StartTimers()
        {
            this.timerOnlineChecking = new Timer(ServerObject.AmountSecondsBetweenOnlineChecking * 1000)
            {
                AutoReset = true, 
                Enabled = true
            };
            this.timerOnlineChecking.Elapsed += this.OnTimerOnlineCheckingTick;
        }
        
        private async Task StartListening()
        {
            while (true)
            {
                try
                {
                    TcpClient client = await this.listener.AcceptTcpClientAsync();
                    IPackage package = this.ReceivePackageAfterConnection(client);

                    if (!this.dictionaryConnectedClients.ContainsKey(package.IdAuthor))
                    {
                        await this.HandleNotConnectedClient(package, client);
                    }
                    else
                    {
                        await this.HandleAlreadyConnectedClient(package, client);
                    }
                }
                catch
                {
                    // TODO Логика работы,в ситуации, когда происходит ошибка при попытке подключения клиента
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private IPackage ReceivePackageAfterConnection(TcpClient client)
        {
            byte[] bytes = new byte[1024];
            
            while (!this.packageCreator.CanGetPackage)
            {
                int amountBytesRead = client.GetStream().Read(bytes, 0, bytes.Length);
                if (amountBytesRead != 0)
                {
                    packageCreator.Add(bytes, amountBytesRead);
                }
            }
            
            return packageCreator.GetPackage();
        }

        private async Task HandleNotConnectedClient(IPackage package, TcpClient client)
        {
            ConnectedClient connectedClient = new ConnectedClient(package.IdAuthor, client.GetStream(), this);
            connectedClient.OnGetPackage += packageReceived =>
                this.OnGetData?.Invoke(packageReceived);
            connectedClient.OnClientDisconnected += () =>
            {
                this.OnClientDisconnected?.Invoke(connectedClient.Id);
                this.dictionaryConnectedClients.Remove(connectedClient.Id);
            };
                    
            this.dictionaryConnectedClients.Add(connectedClient.Id, connectedClient);
            this.OnClientConnected?.Invoke(connectedClient.Id);
                        
            connectedClient.StartListen();
        }

        private async Task HandleAlreadyConnectedClient(IPackage package, TcpClient client)
        {
            // TODO Логика работы в ситуации, когда к серверу пробует подключиться уже подключенный клиент
        }
        
        private async void OnTimerOnlineCheckingTick(object source, ElapsedEventArgs e)
        {
            foreach (ConnectedClient client in this.dictionaryConnectedClients.Values)
            {
                await client.CheckOnline();
            }
        }

        private async Task LoadMessageHistory()
        {
            // TODO Логика работы при загрузке истории
        }

        public async Task SendPackage(IPackage package)
        {
            if (this.dictionaryConnectedClients.ContainsKey(package.IdReceiver))
            {
                await this.dictionaryConnectedClients[package.IdReceiver].SendPackage(package);
            }
        }
        
        public delegate void ServerStartedHandler();
        public event ServerStartedHandler OnServerStarted;
        
        public delegate void ClientConnectedHandler(string id);
        public event ClientConnectedHandler OnClientConnected;
        
        public delegate void ClientDisconnectedHandler(string id);
        public event ClientDisconnectedHandler OnClientDisconnected;
        
        public delegate void GetPackageHandler(IPackage package);
        public event GetPackageHandler OnGetData;
    }
}