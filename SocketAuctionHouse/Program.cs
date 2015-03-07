using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketAuctionHouse
{
    internal class Program
    {
        public static Auction _auction = new Auction();

        private static void Main(string[] args)
        {
            new SocketServer(11100);
        }

        private class SocketServer
        {
            private bool done = true;

            public SocketServer(int port)
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                var server = new TcpListener(ip, port);
                server.Start();

                Console.WriteLine("Serveren is up and kicking : port " + port);

                // Starts a new thread for the auctionhouse
                var auctionThread = new Thread(_auction.RunAuction);
                auctionThread.Start();

                while (true)
                {
                    Socket clientSocket = server.AcceptSocket();
                    Console.WriteLine("A client is connected...");

                    // Init an objekt of ClientHandler
                    var handler = new ClientHandler(clientSocket, _auction);

                    // Starts a new thread for the new client.
                    var clientThread = new Thread(handler.RunClient);
                    clientThread.Start();
                }
            }
        }
    }
}