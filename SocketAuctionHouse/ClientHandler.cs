using System;
using System.IO;
using System.Net.Sockets;

namespace SocketAuctionHouse
{
    internal class ClientHandler
    {
        private readonly Auction _auction;
        private readonly Socket _client;
        private string _clientName;
        private bool done;

        public ClientHandler(Socket client, Auction auction)
        {
            _auction = auction;
            _client = client;

            // connects to the broadcast
            _auction.broadcastEvent += _auction_broadcastEvent;
        }

        private void _auction_broadcastEvent(string message)
        {
            // Setup streams for input and output between the client and the server
            var stream = new NetworkStream(_client);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            // Sends message
            writer.WriteLine(message);
        }

        public void RunClient()
        {
            // Setup streams for input and output between the client and the server
            var stream = new NetworkStream(_client);
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            // Gets the cliens name
            writer.WriteLine("Type your name:");
            _clientName = reader.ReadLine();

            writer.WriteLine("Welcome {0}\r\nwrite 'Bye' to close the connection.\r\n'bid' and a number, to bid on an item.", _clientName);

            // while the client is connected
            done = false;
            while (!done)
            {
                try
                {
                    // Recive command from client
                    string[] commands = reader.ReadLine().Split(' ');

                    // Client commands 
                    switch (commands[0]) // TODO: add more commands
                    {
                        case "Bye":
                            // Disconnects from the broadcast
                            _auction.broadcastEvent -= _auction_broadcastEvent;
                            writer.WriteLine("Thank you for now! Goodbye...");
                            // Sets the boolean because the client is done
                            done = true;
                            break;
                        case "bid":
                            // Makes and bid
                            string bidString = _auction.Bid(_clientName, int.Parse(commands[1]));

                            // Sends the bid text to the client
                            writer.WriteLine(bidString);
                            break;
                        default:

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Expection thrown by ClientHandler - Command(): {0}", ex);
                    done = true;
                }
            }

            // Closes the streams and the socket.
            writer.Close();
            reader.Close();
            stream.Close();
            _client.Close();
        }
    }
}