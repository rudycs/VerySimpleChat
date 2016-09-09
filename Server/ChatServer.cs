using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Server.Model;

namespace Server
{
    public class ChatServer : IChatServer
    {
        private List<ChatClient> ChatClientList { get; set; }
        private TcpListener TcpListener { get; set; }
        private string ServerAddress { get; set; }
        private int ServerPort { get; set; }
        private int LastId { get; set; }

        public ChatServer(string serverAddress, int port)
        {
            ServerAddress = serverAddress;
            ServerPort = port;

            ChatClientList = new List<ChatClient>();
        }

        public void Run()
        {
            try
            {
                LastId = 0;
                Console.WriteLine("Init chat server on {0}:{1}", ServerAddress, ServerPort);
                TcpListener = new TcpListener(IPAddress.Parse(ServerAddress), ServerPort);
                TcpListener.Start();
                Console.WriteLine("Chat server is running");
                Console.WriteLine();

                ListenForConnectionRequest();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Console.ReadLine();
            }
        }

        private void ListenForConnectionRequest()
        {
            while (true)
            {
                // check if any pending connection request
                if (TcpListener.Pending())
                {
                    AcceptConnectionRequest();
                }
            }
        }

        private void AcceptConnectionRequest()
        {
            // create a new connection
            var tcpClient = TcpListener.AcceptTcpClient();
            int newId = ++LastId;
            var newChatClient = new ChatClient { Id = newId, TcpClient = tcpClient };
            SendMessage(0, newChatClient, string.Format("You are now connected. Your id is: {0}", newId));
            BroadcastMessage(0, string.Format("Client {0} connected", newId));
            ChatClientList.Add(newChatClient);
            
            // create a client thread to communicate
            new ClientThread(this, newChatClient);

            Console.WriteLine("New client connected. Id = {0}", newId);
        }

        public void BroadcastMessage(int id, string message)
        {
            // client disconnected
            if (message == null)
            {
                ChatClientList.Remove(ChatClientList.Single(x => x.Id == id));
                message = string.Format("Client {0} disconnected", id);
                Console.WriteLine(message);
                id = 0;
            }

            foreach (var client in ChatClientList)
            {
                SendMessage(id, client, message);
            }
        }

        private void SendMessage(int sourceClientId, ChatClient chatClient, string message)
        {
            var writer = new StreamWriter(chatClient.TcpClient.GetStream());
            var source = sourceClientId == 0 ? "Server" : string.Format("Client {0}", sourceClientId);
            writer.WriteLine(string.Format("{0}: {1}", source, message));
            writer.Flush();
            writer = null;
        }
    }
}
