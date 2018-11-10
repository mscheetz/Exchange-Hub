using Binance.NetCore;
using ExchangeHub.Contracts;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeHub.Proxies
{
    public class BinanceProxy : ProxyBase, IExchangeProxy
    {
        public BinanceApiClient binance;

        public BinanceProxy(Contracts.ApiInformation apiInformation)
        {
            binance = new BinanceApiClient(apiInformation.ApiKey, apiInformation.ApiSecret);
        }

        public IEnumerable<string> GetMarkets()
        {
            throw new Exception("Coming soon");
        }

        public async Task<IEnumerable<string>> GetMarketsAsync()
        {
            throw new Exception("Coming soon");
        }

        public IEnumerable<string> GetMarkets(string baseSymbol)
        {
            throw new Exception("Coming soon");
        }

        public async Task<IEnumerable<string>> GetMarketsAsync(string baseSymbol)
        {
            throw new Exception("Coming soon");
        }

        public IEnumerable<Balance> GetBalance()
        {
            var bal = binance.GetBalance();
            
            return BinanceBalanceToBalance(bal.balances);
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var bal = await binance.GetBalanceAsync();

            return BinanceBalanceToBalance(bal.balances);
        }

        public OrderResponse LimitOrder(string pair, decimal price, decimal quantity, Side side)
        {
            Binance.NetCore.Entities.Side binanceSide = this.BinanceSideReConverter(side);

            var response = binance.LimitOrder(pair, binanceSide, quantity, price);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public async Task<OrderResponse> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side)
        {
            Binance.NetCore.Entities.Side binanceSide = this.BinanceSideReConverter(side);

            var response = await binance.LimitOrderAsync(pair, binanceSide, quantity, price);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public OrderResponse MarketOrder(string pair, decimal quantity, Side side)
        {
            Binance.NetCore.Entities.Side binanceSide = this.BinanceSideReConverter(side);

            var response = binance.MarketOrder(pair, binanceSide, quantity);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public async Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            Binance.NetCore.Entities.Side binanceSide = this.BinanceSideReConverter(side);

            var response = await binance.MarketOrderAsync(pair, binanceSide, quantity);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public OrderResponse StopLossOrder(string pair, decimal quantity, decimal stopPrice, Side side)
        {
            Binance.NetCore.Entities.Side binanceSide = this.BinanceSideReConverter(side);

            var response = binance.StopLoss(pair, binanceSide, quantity, stopPrice);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public async Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal stopPrice, Side side)
        {
            Binance.NetCore.Entities.Side binanceSide = this.BinanceSideReConverter(side);

            var response = await binance.StopLossAsync(pair, binanceSide, quantity, stopPrice);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public OrderResponse CancelOrder(string orderId, string pair)
        {
            var cancelParams = new Binance.NetCore.Entities.CancelTradeParams
            {
                orderId = Int64.Parse(orderId),
                symbol = pair
            };
            var response = binance.DeleteTrade(cancelParams);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public async Task<OrderResponse> CancelOrderAsync(string orderId, string pair)
        {
            var cancelParams = new Binance.NetCore.Entities.CancelTradeParams
            {
                orderId = Int64.Parse(orderId),
                symbol = pair
            };
            var response = await binance.DeleteTradeAsync(cancelParams);

            return this.BinanceTradeResponseToOrderResponse(response);
        }

        public KLine[] GetKLines(string pair, TimeInterval interval, int limit = 20)
        {
            var binanceInterval = this.BinanceIntervalReConverter(interval);

            var response = binance.GetCandlestick(pair, Binance.NetCore.Entities.Interval.EightH, limit);

            return this.BinanceCandlesticksToKLines(response);
        }

        public async Task<KLine[]> GetKLinesAsync(string pair, TimeInterval interval, int limit = 20)
        {
            var binanceInterval = this.BinanceIntervalReConverter(interval);

            var response = await binance.GetCandlestickAsync(pair, Binance.NetCore.Entities.Interval.EightH, limit);

            return this.BinanceCandlesticksToKLines(response);
        }

        public Ticker Get24hrStats(string symbol)
        {
            var response = binance.Get24HourStats(symbol);

            return this.BinanceTickToTicker(response[0]);
        }

        public async Task<Ticker> Get24hrStatsAsync(string symbol)
        {
            var response = await binance.Get24HourStatsAsync(symbol);

            return this.BinanceTickToTicker(response[0]);
        }

        public Dictionary<string, string> GetDepositAddress(string symbol)
        {
            return binance.GetDepositAddress(symbol);
        }

        public async Task<Dictionary<string, string>> GetDepositAddressAsync(string symbol)
        {
            return await binance.GetDepositAddressAsync(symbol);
        }

        public OrderBook GetOrderBook(string symbol, int limit = 100)
        {
            var response = binance.GetOrderBook(symbol, limit);

            return TinyMapper.Map<OrderBook>(response);
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = 100)
        {
            var response = await binance.GetOrderBookAsync(symbol, limit);

            return TinyMapper.Map<OrderBook>(response);
        }

        public OrderResponse GetOrder(string pair, string orderId)
        {
            var response = binance.GetOrder(pair, Int64.Parse(orderId));

            return BinanceOrderResponseToOrderResponse(response);
        }

        public async Task<OrderResponse> GetOrderAsync(string pair, string orderId)
        {
            var response = await binance.GetOrderAsync(pair, Int64.Parse(orderId));

            return BinanceOrderResponseToOrderResponse(response);
        }

        public IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20)
        {
            var response = binance.GetOrders(pair, limit);

            return BinanceOrderResponseCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20)
        {
            var response = await binance.GetOrdersAsync(pair, limit);

            return BinanceOrderResponseCollectionConverter(response);
        }

        public IEnumerable<OrderResponse> GetOpenOrders(string pair)
        {
            var response = binance.GetOpenOrders(pair);

            return this.BinanceOrderResponseCollectionConverter(response);
        }

        public async Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair)
        {
            var response = await binance.GetOpenOrdersAsync(pair);

            return this.BinanceOrderResponseCollectionConverter(response);
        }

        private IEnumerable<OrderResponse> BinanceOrderResponseCollectionConverter(Binance.NetCore.Entities.OrderResponse[] orderResponseArray)
        {
            var orderResponseList = new List<OrderResponse>();

            foreach(var order in orderResponseArray)
            {
                var orderResponse = this.BinanceOrderResponseToOrderResponse(order);
                orderResponseList.Add(orderResponse);
            }

            return orderResponseList;
        }

        public IEnumerable<Balance> BinanceBalanceToBalance(List<Binance.NetCore.Entities.Balance> exchangeBalance)
        {
            var balanceList = new List<Balance>();

            foreach(var exchangeBal in exchangeBalance)
            {
                var balance = TinyMapper.Map<Balance>(exchangeBal);
                balanceList.Add(balance);
            }

            return balanceList;
        }

        public KLine[] BinanceCandlesticksToKLines(Binance.NetCore.Entities.Candlestick[] candlesticks)
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
