using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Very simple chat server");
            Console.WriteLine("=======================");
            Console.Write("Server/IP Address (Leave blank for default - 127.0.0.1): ");
            var server = Console.ReadLine();
            if (string.IsNullOrEmpty(server))
            {
                server = "127.0.0.1";
            }
            Console.Write("Port number (Leave blank for default - 9999): ");
            var portStr = Console.ReadLine();
            int port;
            if (!int.TryParse(portStr, out port))
            {
                port = 9999;
            }

            new ChatServer(server, port).Run();
        }
    }
}
