using System;
using System.Collections.Generic;
using System.Threading;

namespace SocketAuctionHouse
{
    internal class Auction
    {
        public delegate void broadcastDelegate(string message);

        private readonly List<AuctionItem> auctionItems = new List<AuctionItem>();

        private readonly object gravelLock = new object();
        private readonly object itemLock = new object();
        private bool _auctionRunning;
        private AuctionItem _currentAuction;
        private int _gravel;

        public Auction()
        {
            // Adds items for the auction to sell
            auctionItems.Add(new AuctionItem {winner = "No one", startPrice = 100, endPrice = 100, item = "A Pony"});
            auctionItems.Add(new AuctionItem
            {
                winner = "No one",
                startPrice = 100,
                endPrice = 100,
                item = "A forrest in Germany"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "No one",
                startPrice = 100,
                endPrice = 100,
                item = "A bucket of eggs"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "No one",
                startPrice = 100,
                endPrice = 100,
                item = "Two empty cola bottles"
            });
        }

        public event broadcastDelegate broadcastEvent;

        public void RunAuction()
        {
            // Takes the same actions for every item to sell
            foreach (AuctionItem auctionItem in auctionItems)
            {
                _currentAuction = auctionItem;
                _auctionRunning = true;
                ResetGravel();

                // Sending a broadcast that an auction is starting (if there is anyone)
                if (broadcastEvent != null)
                    broadcastEvent("Starting auction for: " + _currentAuction.item + " Starting price: " +
                                   _currentAuction.startPrice);

                while (_auctionRunning)
                {
                    Thread.Sleep(1000);

                    // Locks the _gravel resource
                    lock (gravelLock)
                    {
                        _gravel--;

                        if (_gravel == 10)
                        {
                            if (broadcastEvent != null)
                                broadcastEvent("Gravel: Going once!");
                            Console.WriteLine("Gravel: Going once!");
                        }
                        else if (_gravel == 5)
                        {
                            if (broadcastEvent != null)
                                broadcastEvent("Gravel: Going Twice!");
                            Console.WriteLine("Gravel: Going Twice!");
                        }
                        else if (_gravel == 0)
                        {
                            if (broadcastEvent != null)
                                broadcastEvent("Gravel: Sold!");
                            Console.WriteLine("Gravel: Sold!");
                            _auctionRunning = false;
                        }
                    }
                }
                if (broadcastEvent != null)
                    broadcastEvent(_currentAuction.winner + " won\r\n" + _currentAuction.item + " : " +
                                   _currentAuction.endPrice);
            }
            // Broadcast that the auction is over.
            if (broadcastEvent != null)
                broadcastEvent("All auctions are over - Thank you for trying out our AuctionHouse");
        }


        private void ResetGravel()
        {
            lock (gravelLock)
            {
                _gravel = 30;
            }
        }

        public string Bid(string name, int amount)
        {
            // Locks the current auction item to make a bid.
            lock (itemLock)
            {
                // Checks if the bid is higher then the current
                if (_currentAuction.endPrice < amount)
                {
                    // Broadcast the new bid
                    broadcastEvent(name + " bids " + amount);
                    // Sets the item informations
                    _currentAuction.endPrice = amount;
                    _currentAuction.winner = name;
                    // Reset the _gravel time
                    ResetGravel();
                }
                else
                {
                    return "To low";
                }
            }
            // Returns the information to the client
            return "You Bid : " + amount;
        }
    }

    internal class AuctionItem
    {
        public int startPrice { get; set; }
        public int endPrice { get; set; }
        public string winner { get; set; }
        public string item { get; set; }
    }
}