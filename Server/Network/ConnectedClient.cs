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
        
        private TcpClient client;
        private NetworkStream stream;

        private PackageCreator packageCreator;

        public ConnectedClient(NetworkStream stream)
        {
            this.Online = true;
            this.stream = stream;
            
            this.packageCreator = new PackageCreator();
        }
        
        public ConnectedClient(string id, NetworkStream stream) : this(stream)
        {
            this.Id = id;
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
                int amount = await this.stream.ReadAsync(bytesBuffer);
                this.packageCreator.Add(bytesBuffer, amount);
            }
        }

        private void HandleReceivedPackage()
        {
            IPackage package = this.packageCreator.GetPackage();
            this.OnGetPackage?.Invoke(package);
        }

        public async Task CheckOnline()
        {
            if (!this.Online)
            {
                await this.stream.WriteAsync(new DisconnectPackage(""));
                this.OnClientDisconnected?.Invoke();
            }
            else
            {
                await this.stream.WriteAsync(new OnlinePackage(this.Id, ""));
            }
        }
        
        public delegate void GetPackageHandler(IPackage package);
        public event GetPackageHandler OnGetPackage;
        
        public delegate void ClientDisconnectedHandler();
        public event ClientDisconnectedHandler OnClientDisconnected;
    }
}