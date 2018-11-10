using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeHub.Contracts
{
    public class KLine
    {
        public DateTime OpenTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public DateTime CloseTime { get; set; }
    }
}
