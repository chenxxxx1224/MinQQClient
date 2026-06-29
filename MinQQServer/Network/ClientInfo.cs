using System.Net.Sockets;

namespace MinQQServer.Network
{
    public class ClientInfo
    {
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
