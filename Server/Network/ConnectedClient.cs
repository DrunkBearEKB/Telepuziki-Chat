using System.Net.Sockets;
using System.Threading.Tasks;

using Network.Extensions;
using Network.Package;
using Network.Package.ExchangingPackages;

namespace Server.Network
{
    public class ConnectedClient
    {
        public string Id { get; }
        public bool Online;
        
        private TcpClient client;
        private NetworkStream stream;

        private PackageCreator packageCreator;

        public ConnectedClient(string id, TcpClient client)
        {
            this.Id = id;
            this.Online = true;
            this.client = client;
            this.stream = client.GetStream();
            
            this.packageCreator = new PackageCreator();
        }
        

        public async Task StartListen()
        {
            while (true)
            {
                try
                {
                    await this.ReceivePackage();
                    await this.HandlePackage();
                }
                catch
                {
                    // TODO Логика работы,в ситуации, когда происходит ошибка при попытке получения данных от клиента
                }
            }
        }

        private async Task ReceivePackage()
        {
            byte[] bytesBuffer = new byte[1024];
            
            while (!this.packageCreator.CanGetPackage)
            {
                var amount = await this.stream.ReadAsync(bytesBuffer);
                packageCreator.Add(bytesBuffer, amount);
            }
        }

        private async Task HandlePackage()
        {
            IPackage package = packageCreator.GetPackage();
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