using System;
using System.Text;
using System.Threading.Tasks;

using Network.Package;
using Network.Package.ExchangingPackages;

using Server.Network;

namespace Server
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            ServerObject server = new ServerObject();
            
            server.OnServerStarted += () => Console.WriteLine(">>> Server started!");
            server.OnClientConnected += idClient => Console.WriteLine($">>> {idClient} - connected;");
            server.OnClientDisconnected += idClient => Console.WriteLine($">>> {idClient} - disconnected;");
            server.OnGetData += package =>
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"[PackageType={package.Type}] " +
                               $"[IdAuthor={package.IdAuthor}] " +
                               $"[IdReceiver={package.IdReceiver}]");

                if (package.Type == PackageType.Text)
                {
                    builder.Append(
                        $" [Text={(package as TextPackage)?.Content}] " +
                        $"[Time={(package as TextPackage)?.Time}]");
                }
                
                Console.WriteLine($">>> {builder.ToString()}");
            };

            await server.Start();
        }
    }
}