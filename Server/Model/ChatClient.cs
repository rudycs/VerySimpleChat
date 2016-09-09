using System.Net.Sockets;

namespace Server.Model
{
    public class ChatClient
    {
        public int Id { get; set; }
        public TcpClient TcpClient { get; set; }
    }
}
