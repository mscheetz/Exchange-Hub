using ExchangeHub.Contracts;
using KuCoinApi.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeHub.Proxies
{
    public class KuCoinProxy : ProxyBase, IExchangeProxy
    {
        private KuCoinDotNet kuCoin;

        public KuCoinProxy(ApiInformation apiInformation)
        {
            if(string.IsNullOrEmpty(apiInformation.ApiExtra))
            {
                throw new Exception("Exchange password is missing.");
            }
            kuCoin = new KuCoinDotNet(apiInformation.ApiKey, apiInformation.ApiSecret, apiInformation.ApiExtra);
            this.SetPairs(GetMarkets().ToArray());
        }

        public IEnumerable<string> GetMarkets()
        {
            return this.GetMarketsAsync().Result;
        }

        public async Task<IEnumerable<string>> GetMarketsAsync()
        {
            var response = await kuCoin.GetMarkets();

            return response;
        }

        public IEnumerable<string> GetMarkets(string baseSymbol)
        {
            return this.GetMarketsAsync(baseSymbol).Result;
        }

        public async Task<IEnumerable<string>> GetMarketsAsync(string baseSymbol)
        {
            var response = await kuCoin.GetMarkets();

            return response.Where(r => r.EndsWith(baseSymbol)).ToList();
        }

        public PairPrice GetPrice(string pair)
        {
            return this.GetPriceAsync(pair).Result;
        }

        public async Task<PairPrice> GetPriceAsync(string pair)
        {
            var result = await kuCoin.Get24HrStats(pair);

            return KuCoinStatsToPairPrice(result);
        }

        public IEnumerable<PairPrice> GetPrices()
        {
            return this.GetPricesAsync().Result;
        }

        public async Task<IEnumerable<PairPrice>> GetPricesAsync()
        {
            var tradingPairStats = new List<KuCoinApi.Net.Entities.TradingPairStats>();
            var pairs = await this.GetMarketsAsync();
            foreach (var pair in pairs)
            {
                var stats = await kuCoin.Get24HrStats(pair);
                tradingPairStats.Add(stats);
            }

            return KuCoinStatsCollectionToPairPrice(tradingPairStats);
        }

        public IEnumerable<Balance> GetBalance()
        {
            return this.GetBalanceAsync().Result;
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var response = await kuCoin.GetBalances();

            return KuCoinBalanceCollectionConverter(response);
        }

        public OrderResponse LimitOrder(string pair, decimal price, decimal quantity, Side side)
        {
            return this.LimitOrderAsync(pair, price, quantity, side).Result;
        }

        public async Task<OrderResponse> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side)
        {
            var response = await kuCoin.PlaceLimitOrder(pair, base.KuCoinSideConverter(side), price, quantity);

            return await this.GetOrderAsync(string.Empty, response);
        }

        public OrderResponse MarketOrder(string pair, decimal quantity, Side side)
        {
            return this.MarketOrderAsync(pair, quantity, side).Result;
        }

        public async Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            var response = await kuCoin.PlaceMarketOrder(pair, base.KuCoinSideConverter(side), quantity);

            var orderId = response;

            return await this.GetOrderAsync(pair, orderId);
        }

        public OrderResponse StopLossOrder(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            return this.StopLossOrderAsync(pair, quantity, price, stopPrice, side).Result;
        }

        public async Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            var response = await kuCoin.PlaceStopOrder(pair, base.KuCoinSideConverter(side), price, quantity, stopPrice, KuCoinApi.Net.Entities.StopType.LOSS);
            
            return await this.GetOrderAsync(pair, response);
        }

        public OrderResponse CancelOrder(string orderId, string pair)
        {
            return this.CancelOrderAsync(orderId, pair).Result;
        }

        public async Task<OrderResponse> CancelOrderAsync(string orderId, string pair)
        {
            var response = await kuCoin.CancelOrder(orderId);

            if (response != null)
            {
                var orderResponse = new OrderResponse
                {
                    OrderId = orderId,
                    TransactTime = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Canceled
                };

                return orderResponse;
            }
            return null;
        }

        public KLine[] GetKLines(string pair, TimeInterval interval, int limit = 20)
        {
            return this.GetKLinesAsync(pair, interval, limit).Result;
        }

        public async Task<KLine[]> GetKLinesAsync(string pair, TimeInterval interval, int limit = 20)
        {
            var response = await kuCoin.GetCandlestick(pair, base.KuCoinIntervalConverter(interval), limit);

            return base.KuCoinCandlesticksConverter(response, interval);
        }

        public Ticker Get24hrStats(string symbol)
        {
            return this.Get24hrStatsAsync(symbol).Result;
        }

        public async Task<Ticker> Get24hrStatsAsync(string symbol)
        {
            var tick = await kuCoin.GetTicker(symbol);
            var stats = await kuCoin.Get24HrStats(symbol);

            return this.KuCoinTickToTicker(tick, stats);
        }

        public Dictionary<string, string> GetDepositAddress(string symbol)
        {
            return this.GetDepositAddressAsync(symbol).Result;
        }

        public async Task<Dictionary<string, string>> GetDepositAddressAsync(string symbol)
        {
            var response = await kuCoin.GetDepositAddress(symbol);

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("address", response.Address);
            dictionary.Add("memo", response.Memo);

            return dictionary;
        }

        public OrderBook GetOrderBook(string symbol, int limit = 100)
        {
            return this.GetOrderBookAsync(symbol, limit).Result;
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = 100)
        {
            var response = await kuCoin.GetPartOrderBook(symbol);

            return KuCoinOrderBookConverter(response);
        }

        public OrderResponse GetOrder(string pair, string orderId)
        {
            return this.GetOrderAsync(pair, orderId).Result;
        }

        public async Task<OrderResponse> GetOrderAsync(string pair, string orderId)
        {
            var order = await kuCoin.GetOrder(orderId);

            return this.KuCoinOrderConverter(order);
        }

        public IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20)
        {
            return this.GetOrdersAsync(pair, limit).Result;
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20)
        {
            var response = await kuCoin.GetOrders(pair, limit);

            return KuCoinOrderListDetailCollectionConverter(response.Data);
        }

        public IEnumerable<OrderResponse> GetOpenOrders(string pair)
        {
            return this.GetOpenOrdersAsync(pair).Result;
        }

        public async Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair)
        {
            var response = await kuCoin.GetOpenOrders(pair);

            return KuCoinOrderListDetailCollectionConverter(response.Data);
        }

        private IEnumerable<OrderResponse> KuCoinOrderListDetailCollectionConverter(List<KuCoinApi.Net.Entities.Order> orders)
        {
            var orderResponseList = new List<OrderResponse>();

            foreach(var order in orders)
            {
                var orderResponse = KuCoinOrderConverter(order);

                orderResponseList.Add(orderResponse);
            }

            return orderResponseList;
        }
    }
}
