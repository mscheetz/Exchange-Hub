using Binance.NetCore;
using CoinbaseProApi.NetCore;
using ExchangeHub.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeHub.Proxies
{
    public class CoinbaseProProxy : ProxyBase, IExchangeProxy
    {
        private CoinbaseProClient coinbasePro;

        public CoinbaseProProxy(ApiInformation apiInformation)
        {
            coinbasePro = new CoinbaseProClient(apiInformation.ApiKey, apiInformation.ApiSecret, apiInformation.ApiExtra);
            this.SetPairs(GetMarkets().ToArray());
        }
        
        public IEnumerable<string> GetMarkets()
        {
            return coinbasePro.GetTradingPairs().Select(p => p.id).ToList();
        }

        public async Task<IEnumerable<string>> GetMarketsAsync()
        {
            var pairs = await coinbasePro.GetTradingPairsAsync();

            return pairs.Select(p => p.id).ToList();
        }

        public IEnumerable<string> GetMarkets(string baseSymbol)
        {
            var pairs = coinbasePro.GetTradingPairs();

            return pairs.Where(p => p.id.EndsWith(baseSymbol)).Select(p => p.id).ToList();
        }

        public async Task<IEnumerable<string>> GetMarketsAsync(string baseSymbol)
        {
            var pairs = await coinbasePro.GetTradingPairsAsync();

            return pairs.Where(p => p.id.EndsWith(baseSymbol)).Select(p => p.id).ToList();
        }

        public IEnumerable<Balance> GetBalance()
        {
            var accounts = coinbasePro.GetAccounts();
            
            return CoinbaseProAccountCollectionConverter(accounts);
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var accounts = await coinbasePro.GetAccountsAsync();

            return CoinbaseProAccountCollectionConverter(accounts);
        }

        public OrderResponse LimitOrder(string pair, decimal price, decimal quantity, Side side)
        {
            CoinbaseProApi.NetCore.Entities.SIDE cbpSide = this.CoinbaseProSideReConverter(side);

            var response = coinbasePro.PlaceLimitOrder(cbpSide, pair, quantity, price);

            return this.CoinbaseProOrderResponseConverter(response);
        }

        public async Task<OrderResponse> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side)
        {
            CoinbaseProApi.NetCore.Entities.SIDE cbpSide = this.CoinbaseProSideReConverter(side);

            var response = await coinbasePro.PlaceLimitOrderAsync(cbpSide, pair, quantity, price);

            return this.CoinbaseProOrderResponseConverter(response);
        }

        public OrderResponse MarketOrder(string pair, decimal quantity, Side side)
        {
            CoinbaseProApi.NetCore.Entities.SIDE cbpSide = this.CoinbaseProSideReConverter(side);

            var response = coinbasePro.PlaceMarketOrder(cbpSide, pair, quantity);

            return this.CoinbaseProOrderResponseConverter(response);
        }

        public async Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            CoinbaseProApi.NetCore.Entities.SIDE cbpSide = this.CoinbaseProSideReConverter(side);

            var response = await coinbasePro.PlaceMarketOrderAsync(cbpSide, pair, quantity);

            return this.CoinbaseProOrderResponseConverter(response);
        }

        public OrderResponse StopLossOrder(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            CoinbaseProApi.NetCore.Entities.SIDE cbpSide = this.CoinbaseProSideReConverter(side);

            var response = coinbasePro.PlaceStopOrder(CoinbaseProApi.NetCore.Entities.StopType.LIMIT, cbpSide, pair, price, stopPrice, quantity);

            return this.CoinbaseProOrderResponseConverter(response);
        }

        public async Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            CoinbaseProApi.NetCore.Entities.SIDE cbpSide = this.CoinbaseProSideReConverter(side);

            var response = await coinbasePro.PlaceStopOrderAsync(CoinbaseProApi.NetCore.Entities.StopType.LIMIT, cbpSide, pair, price, stopPrice, quantity);

            return this.CoinbaseProOrderResponseConverter(response);
        }

        public OrderResponse CancelOrder(string orderId, string pair)
        {
            var response = coinbasePro.CancelOrder(orderId);

            var orderResponse = new OrderResponse
            {
                OrderId = orderId,
                TransactTime = DateTime.UtcNow,
                OrderStatus = response ? OrderStatus.Canceled : OrderStatus.Open
            };

            return orderResponse;
        }

        public async Task<OrderResponse> CancelOrderAsync(string orderId, string pair)
        {
            var response = await coinbasePro.CancelOrderAsync(orderId);

            var orderResponse = new OrderResponse
            {
                OrderId = orderId,
                TransactTime = DateTime.UtcNow,
                OrderStatus = response ? OrderStatus.Canceled : OrderStatus.Open
            };

            return orderResponse;
        }

        public KLine[] GetKLines(string pair, TimeInterval interval, int limit = 20)
        {
            throw new Exception("CoinbasePro API does not currently offer a Candlestick/KLine endpoint.");
        }

        public async Task<KLine[]> GetKLinesAsync(string pair, TimeInterval interval, int limit = 20)
        {
            throw new Exception("CoinbasePro API does not currently offer a Candlestick/KLine endpoint.");
        }

        public Ticker Get24hrStats(string pair)
        {
            var response = coinbasePro.GetStats(pair);

            return this.CoinbaseProStatsConverter(response);
        }

        public async Task<Ticker> Get24hrStatsAsync(string pair)
        {
            var response = await coinbasePro.GetStatsAsync(pair);

            return this.CoinbaseProStatsConverter(response);
        }

        public Dictionary<string, string> GetDepositAddress(string symbol)
        {
            throw new Exception("Coinbase Pro API does not currently offer Deposit Addresses.");
        }

        public async Task<Dictionary<string, string>> GetDepositAddressAsync(string symbol)
        {
            throw new Exception("Coinbase Pro API does not currently offer Deposit Addresses.");
        }

        public OrderBook GetOrderBook(string symbol, int limit = 100)
        {
            var response = coinbasePro.GetOrderBook(symbol, 2);

            return CoinbaseProOrderBookResponseConverter(response);
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = 100)
        {
            var response = await coinbasePro.GetOrderBookAsync(symbol, 2);

            return CoinbaseProOrderBookResponseConverter(response);
        }

        public OrderResponse GetOrder(string pair, string orderId)
        {
            var response = coinbasePro.GetOrder(orderId);

            return CoinbaseProOrderConverter(response);
        }

        public async Task<OrderResponse> GetOrderAsync(string pair, string orderId)
        {
            var response = await coinbasePro.GetOrderAsync(orderId);

            return CoinbaseProOrderConverter(response);
        }

        public IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20)
        {
            throw new Exception("Coinbase Pro API does not currently offer all completed orders for an account.");
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20)
        {
            throw new Exception("Coinbase Pro API does not currently offer all completed orders for an account.");
        }

        public IEnumerable<OrderResponse> GetOpenOrders(string pair)
        {
            var response = coinbasePro.GetOrders(pair);

            return CoinbaseProOrderResponseCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair)
        {
            var response = await coinbasePro.GetOrdersAsync(pair);

            return CoinbaseProOrderResponseCollectionConverter(response);
        }
    }
}
