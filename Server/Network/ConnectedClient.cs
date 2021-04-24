using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using Network.Extensions;
using Network.Package;
using Network.Package.ExchangingPackages;

namespace Server.Network
{
    public class ConnectedClient : IDisposable
    {
        public string Id { get; private set; }
        private bool online;
        
        private readonly NetworkStream stream;
        private readonly ServerObject server;

        private PackageCreator packageCreator;

        public ConnectedClient(NetworkStream stream, ServerObject server)
        {
            this.online = true;
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
                    IPackage package = await this.ReceivePackage();
                    await this.HandleReceivedPackage(package);
                }
                catch
                {
                    await this.server.Disconnect(this.Id);
                    await this.Disconnect();
                    
                    // TODO Логика работы,в ситуации, когда происходит ошибка при попытке получения данных от клиента
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task<IPackage> ReceivePackage()
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

            return this.packageCreator.GetPackage();
        }

        private async Task HandleReceivedPackage(IPackage package)
        {
            this.online = true;
            
            if (package.IdReceiver != "")
            {
                await this.server.SendPackage(package);
                this.OnGetPackage?.Invoke(package);
            }
            else
            {
                switch (package)
                {
                    case DisconnectPackage:
                        await this.server.Disconnect(this.Id);
                        await this.Disconnect();
                        break;
                
                    case HistoryRequestPackage historyRequestPackage:
                        // TODO Логика работы в ситуации когда пришёл запрос от клиента на получение истории сообщений
                        break;
                
                    case HistoryAnswerPackage historyAnswerPackage:
                        // TODO Логика работы в ситуации когда пришёл ответ от клиента на получение истории сообщений
                        break;
                }
            }
        }

        public async Task CheckOnline()
        {
            try
            {
                if (!this.online)
                {
                    await this.stream.WriteAsync(new DisconnectPackage(this.Id, ""));
                    this.OnDisconnected?.Invoke();
                }
                else
                {
                    await this.stream.WriteAsync(new OnlinePackage(this.Id, ""));
                    this.online = false;
                }
            }
            catch
            {
                // TODO Логика работы в ситуации когда произошла ошибка связи с клиентом
            }
        }

        public async Task Disconnect()
        {
            await this.SendPackage(new DisconnectPackage(this.Id, ""));
            this.stream.Close();
            this.OnDisconnected?.Invoke();
        }
            
        public delegate void GetPackageHandler(IPackage package);
        public event GetPackageHandler OnGetPackage;
        
        public delegate void ClientDisconnectedHandler();
        public event ClientDisconnectedHandler OnDisconnected;

        public void Dispose()
        {
            stream?.Dispose();
        }
    }
}