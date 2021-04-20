using System;
using System.Threading.Tasks;

using Server.Network;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServerObject server = new ServerObject();
            
            server.OnServerStarted += () => Console.WriteLine("Server started!");
            server.OnClientConnected += idClient => Console.WriteLine($"{idClient} - connected;");
            server.OnClientDisconnected += idClient => Console.WriteLine($"{idClient} - disconnected;");
            server.OnGetData += data => Console.WriteLine(data ?? "Empty data!");

            await server.Start();
        }
    }
}