using BittrexApi.NetCore;
using ExchangeHub.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeHub.Proxies
{
    public class BittrexProxy : ProxyBase, IExchangeProxy
    {
        private BittrexClient bittrex;

        public BittrexProxy(ApiInformation apiInformation)
        {
            bittrex = new BittrexClient(apiInformation.ApiKey, apiInformation.ApiSecret);
            this.SetPairs(GetMarkets().ToArray());
        }

        public IEnumerable<string> GetMarkets()
        {
            var response = bittrex.GetMarkets();

            return response.OrderBy(r => r.pair).Select(r => r.pair).ToList();
        }

        public async Task<IEnumerable<string>> GetMarketsAsync()
        {
            var response = await bittrex.GetMarketsAsync();

            return response.OrderBy(r => r.pair).Select(r => r.pair).ToList();
        }

        public IEnumerable<string> GetMarkets(string baseSymbol)
        {
            var response = bittrex.GetMarkets();

            return response.Where(r => r.pair.EndsWith(baseSymbol)).OrderBy(r => r.pair).Select(r => r.pair).ToList();
        }

        public async Task<IEnumerable<string>> GetMarketsAsync(string baseSymbol)
        {
            var response = await bittrex.GetMarketsAsync();

            return response.Where(r => r.pair.EndsWith(baseSymbol)).OrderBy(r => r.pair).Select(r => r.pair).ToList();
        }

        public IEnumerable<Balance> GetBalance()
        {
            var bal = bittrex.GetBalances();
            
            return BittrexBalanceToBalance(bal);
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var bal = await bittrex.GetBalancesAsync();

            return BittrexBalanceToBalance(bal);
        }

        public OrderResponse LimitOrder(string pair, decimal price, decimal quantity, Side side)
        {
            BittrexApi.NetCore.Entities.Side bittrexSide = this.BittrexSideReConverter(side);
            
            var response = bittrex.LimitOrder(pair, bittrexSide, quantity, price);

            return this.GetOrder(string.Empty, response);
        }

        public async Task<OrderResponse> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side)
        {
            BittrexApi.NetCore.Entities.Side bittrexSide = this.BittrexSideReConverter(side);

            var response = await bittrex.LimitOrderAsync(pair, bittrexSide, quantity, price);

            return this.GetOrder(string.Empty, response);
        }

        public OrderResponse MarketOrder(string pair, decimal quantity, Side side)
        {
            BittrexApi.NetCore.Entities.Side bittrexSide = this.BittrexSideReConverter(side);

            var response = bittrex.MarketOrder(pair, bittrexSide, quantity);

            return this.GetOrder(string.Empty, response);
        }

        public async Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            BittrexApi.NetCore.Entities.Side bittrexSide = this.BittrexSideReConverter(side);

            var response = await bittrex.MarketOrderAsync(pair, bittrexSide, quantity);

            return this.GetOrder(string.Empty, response);
        }

        public OrderResponse StopLossOrder(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            throw new Exception("Bittrex Api does not offer Stop-Loss orders");
        }

        public async Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            throw new Exception("Bittrex Api does not offer Stop-Loss orders");
        }

        public OrderResponse CancelOrder(string orderId, string pair)
        {
            var response = bittrex.CancelOrder(orderId);

            var orderResponse = new OrderResponse
            {
                OrderId = orderId,
                TransactTime = DateTime.UtcNow,
                OrderStatus = OrderStatus.Canceled
            };

            return response ? orderResponse : null;
        }

        public async Task<OrderResponse> CancelOrderAsync(string orderId, string pair)
        {
            var response = await bittrex.CancelOrderAsync(orderId);

            var orderResponse = new OrderResponse
            {
                OrderId = orderId,
                TransactTime = DateTime.UtcNow,
                OrderStatus = OrderStatus.Canceled
            };

            return response ? orderResponse : null;
        }

        public KLine[] GetKLines(string pair, TimeInterval interval, int limit = 20)
        {
            throw new Exception("Bittrex Api does not offer Candlestick/Kline information");
        }

        public async Task<KLine[]> GetKLinesAsync(string pair, TimeInterval interval, int limit = 20)
        {
            throw new Exception("Bittrex Api does not offer Candlestick/Kline information");
        }

        public Ticker Get24hrStats(string symbol)
        {
            var response = bittrex.GetMarketSummary(symbol);

            return BittrexMarketSummaryConverter(response);
        }

        public async Task<Ticker> Get24hrStatsAsync(string symbol)
        {
            var response = await bittrex.GetMarketSummaryAsync(symbol);

            return BittrexMarketSummaryConverter(response);
        }

        public Dictionary<string, string> GetDepositAddress(string symbol)
        {
            var response = bittrex.GetDepositAddress(symbol);

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("address", response);

            return dictionary;
        }

        public async Task<Dictionary<string, string>> GetDepositAddressAsync(string symbol)
        {
            var response = await bittrex.GetDepositAddressAsync(symbol);

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("address", response);

            return dictionary;
        }

        public OrderBook GetOrderBook(string symbol, int limit = 100)
        {
            var response = bittrex.GetOrderBook(symbol);

            return BittrexOrderBookConverter(response);
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = 100)
        {
            var response = await bittrex.GetOrderBookAsync(symbol);

            return BittrexOrderBookConverter(response);
        }

        public OrderResponse GetOrder(string pair, string orderId)
        {
            var response = bittrex.GetOrder(orderId);

            return this.BittrexOrderDetailToOrderResponse(response);
        }

        public async Task<OrderResponse> GetOrderAsync(string pair, string orderId)
        {
            var response = await bittrex.GetOrderAsync(orderId);

            return this.BittrexOrderDetailToOrderResponse(response);
        }

        public IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20)
        {
            var response = bittrex.GetOrderHistory(pair);

            return this.BittrexOrderCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20)
        {
            var response = await bittrex.GetOrderHistoryAsync(pair);

            return BittrexOrderCollectionConverter(response);
        }

        public IEnumerable<OrderResponse> GetOpenOrders(string pair)
        {
            var response = bittrex.GetOpenOrders(pair);

            return this.BittrexOpenOrderCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair)
        {
            var response = await bittrex.GetOpenOrdersAsync(pair);

            return this.BittrexOpenOrderCollectionConverter(response);
        }

        private IEnumerable<OrderResponse> BittrexOrderCollectionConverter(BittrexApi.NetCore.Entities.Order[] orderResponseArray)
        {
            var orderResponseList = new List<OrderResponse>();

            foreach(var order in orderResponseArray)
            {
                var orderResponse = this.BittrexOrderToOrderResponse(order);
                orderResponseList.Add(orderResponse);
            }

            return orderResponseList;
        }

        private IEnumerable<OrderResponse> BittrexOpenOrderCollectionConverter(BittrexApi.NetCore.Entities.OpenOrder[] orderResponseArray)
        {
            var orderResponseList = new List<OrderResponse>();

            foreach (var order in orderResponseArray)
            {
                var orderResponse = this.BittrexOpenOrderToOrderResponse(order);
                orderResponseList.Add(orderResponse);
            }

            return orderResponseList;
        }

        public IEnumerable<Balance> BittrexBalanceToBalance(BittrexApi.NetCore.Entities.Balance[] exchangeBalance)
        {
            var balanceList = new List<Balance>();

            foreach(var exchangeBal in exchangeBalance)
            {
                var balance = BittrexBalanceConverter(exchangeBal);
                balanceList.Add(balance);
            }

            return balanceList;
        }

        public KLine[] BittrexCandlesticksToKLines(Binance.NetCore.Entities.Candlestick[] candlesticks)
        {
            var klineList = new List<KLine>();

            foreach(var candlestick in candlesticks)
            {
                var kline = this.BinanceCandlestickToKLine(candlestick);

                klineList.Add(kline);
            }

            return klineList.ToArray();
        }
    }
}
