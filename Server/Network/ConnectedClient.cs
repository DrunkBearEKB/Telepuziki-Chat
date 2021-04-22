using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using Network.Extensions;
using Network.Package;
using Network.Package.ExchangingPackages;

namespace Server.Network
{
    public class ConnectedClient
    {
        public string Id { get; private set; }
        public bool Online { get; private set; }
        
        private NetworkStream stream;
        private ServerObject server;

        private PackageCreator packageCreator;

        public ConnectedClient(NetworkStream stream, ServerObject server)
        {
            this.Online = true;
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
            await this.stream.WriteAsync(package);
        }

        public async Task StartListen()
        {
            while (true)
            {
                try
                {
                    await this.ReceivePackage();
                    this.HandleReceivedPackage();
                }
                catch
                {
                    // TODO Логика работы,в ситуации, когда происходит ошибка при попытке получения данных от клиента
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task ReceivePackage()
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
        }

        private async Task HandleReceivedPackage()
        {
            IPackage package = this.packageCreator.GetPackage();

            if (package.IdReceiver != "")
            {
                await this.server.SendPackage(package);
            }
            
            this.OnGetPackage?.Invoke(package);
            this.Online = true;
        }

        public async Task CheckOnline()
        {
            try
            {
                if (!this.Online)
                {
                    await this.stream.WriteAsync(new DisconnectPackage(this.Id, ""));
                    this.OnClientDisconnected?.Invoke();
                }
                else
                {
                    await this.stream.WriteAsync(new OnlinePackage(this.Id, ""));
                    this.Online = false;
                }
            }
            catch
            {
                // TODO Логика работы в ситуации когда произошла ошибка связи с клиентом
            }
        }

        public async Task Disconnect()
        {
            this.OnClientDisconnected?.Invoke();
        }
            
        public delegate void GetPackageHandler(IPackage package);
        public event GetPackageHandler OnGetPackage;
        
        public delegate void ClientDisconnectedHandler();
        public event ClientDisconnectedHandler OnClientDisconnected;
    }
}