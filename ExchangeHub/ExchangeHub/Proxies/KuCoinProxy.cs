using BittrexApi.NetCore;
using ExchangeHub.Contracts;
using KuCoinApi.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeHub.Proxies
{
    public class KuCoinProxy : ProxyBase, IExchangeProxy
    {
        public KuCoinApiClient kuCoin;

        public KuCoinProxy(ApiInformation apiInformation)
        {
            kuCoin = new KuCoinApiClient(apiInformation.ApiKey, apiInformation.ApiSecret);
        }

        public IEnumerable<string> GetMarkets()
        {
            var response = kuCoin.GetMarkets();

            return response.ToList();
        }

        public async Task<IEnumerable<string>> GetMarketsAsync()
        {
            var response = await kuCoin.GetMarketsAsync();

            return response.ToList();
        }

        public IEnumerable<string> GetMarkets(string baseSymbol)
        {
            var response = kuCoin.GetMarkets();

            return response.Where(r => r.EndsWith(baseSymbol)).ToList();
        }

        public async Task<IEnumerable<string>> GetMarketsAsync(string baseSymbol)
        {
            var response = await kuCoin.GetMarketsAsync();

            return response.Where(r => r.EndsWith(baseSymbol)).ToList();
        }

        public IEnumerable<Balance> GetBalance()
        {
            var response = kuCoin.GetBalances();

            return KuCoinBalanceCollectionConverter(response);
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var response = await kuCoin.GetBalancesAsync();

            return KuCoinBalanceCollectionConverter(response);
        }

        public OrderResponse LimitOrder(string pair, decimal price, decimal quantity, Side side)
        {
            var response = kuCoin.LimitOrder(pair, price, quantity, base.KuCoinSideConverter(side));
            
            return this.GetOrder(string.Empty, response.data["orderOid"]);
        }

        public async Task<OrderResponse> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side)
        {
            var response = await kuCoin.LimitOrderAsync(pair, price, quantity, base.KuCoinSideConverter(side));

            return await this.GetOrderAsync(string.Empty, response.data["orderOid"]);
        }

        public OrderResponse MarketOrder(string pair, decimal quantity, Side side)
        {
            var response = kuCoin.MarketOrder(pair, quantity, base.KuCoinSideConverter(side));

            var orderId = response.data["orderOid"];

            return this.GetOrder(pair, orderId);
        }

        public async Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            var response = await kuCoin.MarketOrderAsync(pair, quantity, base.KuCoinSideConverter(side));

            var orderId = response.data["orderOid"];

            return await this.GetOrderAsync(pair, orderId);
        }

        public OrderResponse StopLossOrder(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            throw new Exception("KuCoin Api does not offer Stop-Loss orders");
        }

        public async Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            throw new Exception("KuCoin Api does not offer Stop-Loss orders");
        }

        public OrderResponse CancelOrder(string orderId, string pair)
        {
            var buyResponse = kuCoin.DeleteTrade(pair, orderId, Side.Buy.ToString().ToUpper());
            var sellResponse = kuCoin.DeleteTrade(pair, orderId, Side.Sell.ToString().ToUpper());
            
            var orderResponse = new OrderResponse
            {
                OrderId = orderId,
                TransactTime = DateTime.UtcNow,
                OrderStatus = OrderStatus.Canceled
            };

            return buyResponse.success || sellResponse.success ? orderResponse : null;
        }

        public async Task<OrderResponse> CancelOrderAsync(string orderId, string pair)
        {
            var buyResponse = await kuCoin.DeleteTradeAsync(pair, orderId, Side.Buy.ToString().ToUpper());
            var sellResponse = await kuCoin.DeleteTradeAsync(pair, orderId, Side.Sell.ToString().ToUpper());

            var orderResponse = new OrderResponse
            {
                OrderId = orderId,
                TransactTime = DateTime.UtcNow,
                OrderStatus = OrderStatus.Canceled
            };

            return buyResponse.success || sellResponse.success ? orderResponse : null;
        }

        public KLine[] GetKLines(string pair, TimeInterval interval, int limit = 20)
        {
            var response = kuCoin.GetCandlesticks(pair, base.KuCoinIntervalConverter(interval), limit);

            return base.KuCoinChartValueConverter(response);
        }

        public async Task<KLine[]> GetKLinesAsync(string pair, TimeInterval interval, int limit = 20)
        {
            var response = await kuCoin.GetCandlesticksAsync(pair, base.KuCoinIntervalConverter(interval), limit);

            return base.KuCoinChartValueConverter(response);
        }

        public Ticker Get24hrStats(string symbol)
        {
            var response = kuCoin.GetTick(symbol);

            return this.KuCoinTickToTicker(response);
        }

        public async Task<Ticker> Get24hrStatsAsync(string symbol)
        {
            var response = await kuCoin.GetTickAsync(symbol);

            return this.KuCoinTickToTicker(response);
        }

        public Dictionary<string, string> GetDepositAddress(string symbol)
        {
            var response = kuCoin.GetDepositAddress(symbol);

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("address", response);

            return dictionary;
        }

        public async Task<Dictionary<string, string>> GetDepositAddressAsync(string symbol)
        {
            var response = await kuCoin.GetDepositAddressAsync(symbol);

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("address", response);

            return dictionary;
        }

        public OrderBook GetOrderBook(string symbol, int limit = 100)
        {
            var response = kuCoin.GetOrderBook(symbol);

            return KuCoinOrderBookResponseConverter(response);
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = 100)
        {
            var response = await kuCoin.GetOrderBookAsync(symbol);

            return KuCoinOrderBookResponseConverter(response);
        }

        public OrderResponse GetOrder(string pair, string orderId)
        {
            var buyOrder = kuCoin.GetOrder(pair, KuCoinApi.NetCore.Entities.TradeType.BUY, Int64.Parse(orderId));

            if (buyOrder != null)
            {
                return this.KuCoinOrderListDetailConverter(buyOrder);
            }
            else
            {
                var sellOrder = kuCoin.GetOrder(pair, KuCoinApi.NetCore.Entities.TradeType.SELL, Int64.Parse(orderId));

                return this.KuCoinOrderListDetailConverter(sellOrder);
            }
        }

        public async Task<OrderResponse> GetOrderAsync(string pair, string orderId)
        {
            var buyOrder = await kuCoin.GetOrderAsync(pair, KuCoinApi.NetCore.Entities.TradeType.BUY, Int64.Parse(orderId));

            if (buyOrder != null)
            {
                return this.KuCoinOrderListDetailConverter(buyOrder);
            }
            else
            {
                var sellOrder = await kuCoin.GetOrderAsync(pair, KuCoinApi.NetCore.Entities.TradeType.SELL, Int64.Parse(orderId));
                return this.KuCoinOrderListDetailConverter(sellOrder);
            }
        }

        public IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20)
        {
            var response = kuCoin.GetOrders(pair, limit);

            return this.KuCoinOrderListDetailCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20)
        {
            var response = await kuCoin.GetOrdersAsync(pair, limit);

            return KuCoinOrderListDetailCollectionConverter(response);
        }

        public IEnumerable<OrderResponse> GetOpenOrders(string pair)
        {
            var response = kuCoin.GetOpenOrders(pair);

            return this.KuCoinOpenOrderResponseConverter(response, pair);
        }

        public async Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair)
        {
            var response = await kuCoin.GetOpenOrdersAsync(pair);

            return this.KuCoinOpenOrderResponseConverter(response, pair);
        }

        private IEnumerable<OrderResponse> KuCoinOrderListDetailCollectionConverter(KuCoinApi.NetCore.Entities.OrderListDetail[] oldArray)
        {
            var orderResponseList = new List<OrderResponse>();

            foreach(var old in oldArray)
            {
                var orderResponse = KuCoinOrderListDetailConverter(old);

                orderResponseList.Add(orderResponse);
            }

            return orderResponseList;
        }
    }
}
