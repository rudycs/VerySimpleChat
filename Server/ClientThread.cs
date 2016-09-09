using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

using Server.Model;

namespace Server
{
    public class ClientThread
    {
        private IChatServer ChatServer { get; set; }
        private ChatClient ChatClient { get; set; }
        private StreamReader Reader { get; set; }

        public ClientThread(IChatServer chatServer, ChatClient chatClient)
        {
            ChatServer = chatServer;
            ChatClient = chatClient;

            new Thread(new ThreadStart(StartChat)).Start();
        }

        private void StartChat()
        {
            Reader = new StreamReader(ChatClient.TcpClient.GetStream());

            // create a new thread for this user
            new Thread(new ThreadStart(RunChat)).Start();
        }

        private void RunChat()
        {
            //try
            //{
                var line = string.Empty;
                while (true)
                {
                    line = Reader.ReadLine();
                    ChatServer.BroadcastMessage(ChatClient.Id, line);
                    if (line == null)
                    {
                        break;
                    }
                }
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception);
            //}
        }
    }
}
