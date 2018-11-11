using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeHub.Contracts
{
    public class OrderResponse
    {
        public string Pair { get; set; }
        public string OrderId { get; set; }
        public DateTime TransactTime { get; set; }
        public decimal Price { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal FilledQuantity { get; set; }
        public decimal StopPrice { get; set; }
        public Side Side { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
