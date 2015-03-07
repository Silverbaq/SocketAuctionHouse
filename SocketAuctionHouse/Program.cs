using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketAuctionHouse
{
    class Program
    {
        public static Auction _auction = new Auction();

        static void Main(string[] args)
        {
            new SocketServer(11100);
        }

        class SocketServer
        {
            bool done = true;

            public SocketServer(int port)
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                TcpListener server = new TcpListener(ip, port);
                server.Start();

                Console.WriteLine("Serveren is up and kicking : port " + port);

                // Starts a new thread for the auctionhouse
                Thread auctionThread = new Thread(_auction.RunAuction);
                auctionThread.Start();

                while (true)
                {
                    Socket clientSocket = server.AcceptSocket();
                    Console.WriteLine("A client is connected...");

                    // Init an objekt of ClientHandler
                    ClientHandler handler = new ClientHandler(clientSocket, _auction);

                    // Starts a new thread for the new client.
                    Thread clientThread = new Thread(handler.RunClient);
                    clientThread.Start();
                }

            }
        }

    }
}
