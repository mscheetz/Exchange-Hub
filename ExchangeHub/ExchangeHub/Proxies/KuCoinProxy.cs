using BittrexApi.NetCore;
using ExchangeHub.Contracts;
using KuCoinApi.NetCore;
using Nelibur.ObjectMapper;
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

        public KuCoinProxy(Contracts.ApiInformation apiInformation)
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
            throw new Exception("Coming soon");
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            throw new Exception("Coming soon");
        }

        public OrderResponse LimitOrder(string pair, decimal price, decimal quantity, Side side)
        {
            var parms = new KuCoinApi.NetCore.Entities.TradeParams
            {
                price = price,
                quantity = quantity,
                side = side.ToString().ToUpper(),
                symbol = pair
            };

            var response = kuCoin.PostTrade(parms);
            
            return this.GetOrder(string.Empty, response.data["orderOid"]);
        }

        public async Task<OrderResponse> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side)
        {
            var parms = new KuCoinApi.NetCore.Entities.TradeParams
            {
                price = price,
                quantity = quantity,
                side = side.ToString().ToUpper(),
                symbol = pair
            };

            var response = await kuCoin.PostTradeAsync(parms);

            return await this.GetOrderAsync(string.Empty, response.data["orderOid"]);
        }

        public OrderResponse MarketOrder(string pair, decimal quantity, Side side)
        {
            var price = kuCoin.GetTicker(pair).last;

            BittrexApi.NetCore.Entities.Side kuCoinSide = this.BittrexSideReConverter(side);

            var response = kuCoin.PlaceOrder(pair, kuCoinSide, quantity, price);

            return this.GetOrder(string.Empty, response);
        }

        public async Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            var ticker = await kuCoin.GetTickerAsync(pair);
            var price = ticker.last;

            BittrexApi.NetCore.Entities.Side kuCoinSide = this.BittrexSideReConverter(side);

            var response = await kuCoin.PlaceOrderAsync(pair, kuCoinSide, quantity, price);

            return this.GetOrder(string.Empty, response);
        }

        public OrderResponse StopLossOrder(string pair, decimal quantity, decimal stopPrice, Side side)
        {
            throw new Exception("Bittrex Api does not offer Stop-Loss orders");
        }

        public async Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal stopPrice, Side side)
        {
            throw new Exception("Bittrex Api does not offer Stop-Loss orders");
        }

        public OrderResponse CancelOrder(string orderId, string pair)
        {
            var response = kuCoin.CancelOrder(orderId);

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
            var response = await kuCoin.CancelOrderAsync(orderId);

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

            return TinyMapper.Map<OrderBook>(response);
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = 100)
        {
            var response = await kuCoin.GetOrderBookAsync(symbol);

            return TinyMapper.Map<OrderBook>(response);
        }

        public OrderResponse GetOrder(string pair, string orderId, Side side)
        {
            var tradeType = KuCoinTradeTypeReConverter(side);

            var response = kuCoin.GetOrder(pair, tradeType, Int64.Parse(orderId));

            return this.BittrexOrderDetailToOrderResponse(response);
        }

        public async Task<OrderResponse> GetOrderAsync(string pair, string orderId, Side side)
        {
            var tradeType = KuCoinTradeTypeReConverter(side);

            var response = await kuCoin.GetOrderAsync(pair, tradeType, Int64.Parse(orderId));

            return this.BittrexOrderDetailToOrderResponse(response);
        }

        public IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20)
        {
            var response = kuCoin.GetOrderHistory(pair);

            return this.BittrexOrderCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20)
        {
            var response = await kuCoin.GetOrderHistoryAsync(pair);

            return BittrexOrderCollectionConverter(response);
        }

        public IEnumerable<OrderResponse> GetOpenOrders(string pair)
        {
            var response = kuCoin.GetOpenOrders(pair);

            return this.BittrexOpenOrderCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair)
        {
            var response = await kuCoin.GetOpenOrdersAsync(pair);

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
                var balance = TinyMapper.Map<Balance>(exchangeBal);
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
