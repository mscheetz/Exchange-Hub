using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeHub.Contracts
{
    public class OrderBook
    {
        public Order[] asks { get; set; }
        public Order[] bids { get; set; }
    }
}
