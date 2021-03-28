using System.Net.Sockets;

namespace NetworkLibrary.Server
{
    public class ConnectedClient: TcpClient
    {
        public bool Online { get; set; }
    }
}