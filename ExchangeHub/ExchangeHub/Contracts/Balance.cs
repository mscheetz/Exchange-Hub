using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeHub.Contracts
{
    public class Balance
    {
        public string Symbol { get; set; }
        public decimal Available { get; set; }
        public decimal Frozen { get; set; }
        public decimal Total
        {
            get
            {
                return Available + Frozen;
            }
        }
    }
}
