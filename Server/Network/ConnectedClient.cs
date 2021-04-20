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

        public ConnectedClient(string id, TcpClient client)
        {
            this.Id = id;
            this.Online = true;
            this.client = client;
            this.stream = client.GetStream();
        }
        

        public async Task StartListen()
        {
            PackageCreator packageCreator = new PackageCreator();

            byte[] bytesBuffer = new byte[1024];

            while (true)
            {
                try
                {
                    while (!packageCreator.CanGetPackage)
                    {
                        var amount = await this.stream.ReadAsync(bytesBuffer);
                        packageCreator.Add(bytesBuffer, amount);
                    }
                    IPackage package = packageCreator.GetPackage();
                    this.OnGetPackage?.Invoke(package);
                }
                catch
                {
                    // TODO Логика работы,в ситуации, когда происходит ошибка при попытке получения данных от клиента
                }
            }
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