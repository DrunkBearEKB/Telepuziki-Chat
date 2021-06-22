using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DataBase;
using Network.Package;
using Network.Package.ExchangingPackages;
using Server.Network;

namespace Server
{
    static class Program
    {
        public static async Task Main(string[] args)
        {
            ServerObject server = new ServerObject();
            
            server.OnStarted += () => Console.WriteLine(">>> Server started!");
            server.OnClientConnected += idClient => Console.WriteLine($">>> {idClient} - connected;");
            server.OnClientDisconnected += idClient => Console.WriteLine($">>> {idClient} - disconnected;");
            server.OnGetData += package =>
            {
                if (package.Type != PackageType.Online)
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
                }
            };

            await server.Start();
        }
    }
}