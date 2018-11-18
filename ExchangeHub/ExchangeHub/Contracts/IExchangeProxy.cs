using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeHub.Contracts
{
    public interface IExchangeProxy
    {
        IEnumerable<string> GetMarkets();

        Task<IEnumerable<string>> GetMarketsAsync();

        IEnumerable<string> GetMarkets(string baseSymbol);

        Task<IEnumerable<string>> GetMarketsAsync(string baseSymbol);

        IEnumerable<Balance> GetBalance();

        Task<IEnumerable<Balance>> GetBalanceAsync();

        OrderResponse LimitOrder(string pair, decimal price, decimal quantity, Side side);

        Task<OrderResponse> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side);

        OrderResponse MarketOrder(string pair, decimal quantity, Side side);

        Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side);

        OrderResponse StopLossOrder(string pair, decimal quantity, decimal price, decimal stopPrice, Side side);

        Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal price, decimal stopPrice, Side side);

        OrderResponse CancelOrder(string orderId, string pair);

        Task<OrderResponse> CancelOrderAsync(string orderId, string pair);

        KLine[] GetKLines(string pair, TimeInterval interval, int limit = 20);

        Task<KLine[]> GetKLinesAsync(string pair, TimeInterval interval, int limit = 20);

        Ticker Get24hrStats(string symbol);

        Task<Ticker> Get24hrStatsAsync(string symbol);

        Dictionary<string, string> GetDepositAddress(string symbol);

        Task<Dictionary<string, string>> GetDepositAddressAsync(string symbol);

        OrderBook GetOrderBook(string symbol, int limit = 100);

        Task<OrderBook> GetOrderBookAsync(string symbol, int limit = 100);

        OrderResponse GetOrder(string pair, string orderId);

        Task<OrderResponse> GetOrderAsync(string pair, string orderId);

        IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20);

        Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20);

        IEnumerable<OrderResponse> GetOpenOrders(string pair);

        Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair);
    }
}
