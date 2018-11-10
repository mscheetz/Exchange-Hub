using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeHub.Contracts
{
    public class Ticker
    {
        public DateTime CloseTime { get; set; }
        public DateTime OpenTime { get; set; }
        public decimal Volume { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Open { get; set; }
        public decimal AskQty { get; set; }
        public decimal AskPrice { get; set; }
        public decimal BidQty { get; set; }
        public decimal BidPrice { get; set; }
        public decimal LastQty { get; set; }
        public decimal LastPrice { get; set; }
        public decimal PreviousClosePrice { get; set; }
        public decimal WeightedAvgPrice { get; set; }
        public double PriceChangePercent { get; set; }
        public decimal PriceChange { get; set; }
        public string Pair { get; set; }
    }
}
