using ExchangeHub.Contracts;
using ExchangeHub.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeHub
{
    /// <summary>
    /// Represents ExchangeHub entry
    /// </summary>
    public class ExchangeHub
    {
        private IExchangeProxy proxy;
        private Exchange _exchange;
        private List<string> _markets;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exchange">Exchange to access</param>
        /// <param name="WIF">Wallet private key WIF</param>
        public ExchangeHub(Exchange exchange, string WIF)
        {
            LoadExchange(exchange, WIF, string.Empty, string.Empty);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exchange">Exchange to access</param>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        public ExchangeHub(Exchange exchange, string apiKey, string apiSecret)
        {
            LoadExchange(exchange, apiKey, apiSecret, string.Empty);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exchange">Exchange to access</param>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        /// <param name="apiPassword">Api password value</param>
        public ExchangeHub(Exchange exchange, string apiKey, string apiSecret, string apiPassword)
        {
            LoadExchange(exchange, apiKey, apiSecret, apiPassword);
        }

        /// <summary>
        /// Load an exchange
        /// </summary>
        /// <param name="exchange">Exchange to access</param>
        /// <param name="apiKeyWIF">Api key/WIF</param>
        /// <param name="apiSecret">Api secret</param>
        /// <param name="apiPassword">Api password value</param>
        private void LoadExchange(Exchange exchange, string apiKeyWIF, string apiSecret, string apiPassword)
        {
            var apiInfo = new ApiInformation
            {
                ApiKey = apiKeyWIF,
                ApiSecret = apiSecret,
                ApiExtra = apiPassword
            };

            this._exchange = exchange;
            this._markets = new List<string>();

            if (exchange == Exchange.Binance)
            {
                proxy = new BinanceProxy(apiInfo);
            }
            else if (exchange == Exchange.Bittrex)
            {
                proxy = new BittrexProxy(apiInfo);
            }
            else if (exchange == Exchange.CoinbasePro)
            {
                proxy = new CoinbaseProProxy(apiInfo);
            }
            else if (exchange == Exchange.CoinEx)
            {
                throw new Exception("CoinEx coming soon!");
            }
            else if (exchange == Exchange.KuCoin)
            {
                proxy = new KuCoinProxy(apiInfo);
            }
            else if (exchange == Exchange.Switcheo)
            {
                throw new Exception("Switcheo coming soon!");
            }
        }

        /// <summary>
        /// Get currently loaded exchange
        /// </summary>
        /// <returns>Exchange value</returns>
        public Exchange GetExchange()
        {
            return this._exchange;
        }

        /// <summary>
        /// Reload the exchange hub
        /// </summary>
        /// <param name="exchange">Exchange to access</param>
        /// <param name="WIF">Wallet private key WIF</param>
        /// <returns>Boolean when complete</returns>
        public bool ReloadExchangeHub(Exchange exchange, string WIF)
        {
            LoadExchange(exchange, WIF, string.Empty, string.Empty);
            return true;
        }

        /// <summary>
        /// Reload the exchange hub
        /// </summary>
        /// <param name="exchange">Exchange to access</param>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        /// <returns>Boolean when complete</returns>
        public bool ReloadExchangeHub(Exchange exchange, string apiKey, string apiSecret)
        {
            LoadExchange(exchange, apiKey, apiSecret, string.Empty);
            return true;
        }

        /// <summary>
        /// Reload the exchange hub
        /// </summary>
        /// <param name="exchange">Exchange to access</param>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        /// <param name="apiExtra">Api extra value</param>
        /// <returns>Boolean when complete</returns>
        public bool ReloadExchangeHub(Exchange exchange, string apiKey, string apiSecret, string apiExtra)
        {
            LoadExchange(exchange, apiKey, apiSecret, apiExtra);
            return true;
        }

        /// <summary>
        /// Sets markets in the hub
        /// </summary>
        public void SetMarkets()
        {
            this._markets = proxy.GetMarkets().ToList();
        }

        /// <summary>
        /// Sets markets in the hub
        /// </summary>
        public async Task SetMarketsAsync()
        {
            var mkts = await proxy.GetMarketsAsync();

            this._markets = mkts.ToList();
        }

        /// <summary>
        /// Get all markets
        /// </summary>
        /// <returns>Collection of strings</returns>
        public IEnumerable<string> GetMarkets()
        {
            if (_markets.Count == 0)
            {
                return proxy.GetMarkets();
            }
            else
            {
                return _markets;
            }
        }

        /// <summary>
        /// Get all markets
        /// </summary>
        /// <returns>Collection of strings</returns>
        public async Task<IEnumerable<string>> GetMarketsAsync()
        {
            if (_markets.Count == 0)
            {
                return await proxy.GetMarketsAsync();
            }
            else
            {
                return _markets;
            }
        }

        /// <summary>
        /// Get all markets for a base symbol
        /// </summary>
        /// <param name="baseSymbol">Base trading symbol</param>
        /// <returns>Collection of strings</returns>
        public IEnumerable<string> GetMarkets(string baseSymbol)
        {
            baseSymbol = proxy.FormatPair(baseSymbol, Exchange.KuCoin);
            if (_markets.Count == 0)
            {
                return proxy.GetMarkets(baseSymbol);
            }
            else
            {
                return _markets.Where(m => m.EndsWith(baseSymbol)).ToList();
            }
        }

        /// <summary>
        /// Get all markets for a base symbol
        /// </summary>
        /// <param name="baseSymbol">Base trading symbol</param>
        /// <returns>Collection of strings</returns>
        public async Task<IEnumerable<string>> GetMarketsAsync(string baseSymbol)
        {
            baseSymbol = proxy.FormatPair(baseSymbol, Exchange.KuCoin);
            if (_markets.Count == 0)
            {
                return await proxy.GetMarketsAsync(baseSymbol);
            }
            else
            {
                return _markets.Where(m => m.EndsWith(baseSymbol)).ToList();
            }
        }

        /// <summary>
        /// Get Balance of exchange account
        /// </summary>
        /// <returns>Collection of Balance objects</returns>
        public IEnumerable<Balance> GetBalance()
        {
            return proxy.GetBalance();
        }

        /// <summary>
        /// Get Balance of exchange account
        /// </summary>
        /// <returns>Collection of Balance objects</returns>
        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            return await proxy.GetBalanceAsync();
        }

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <param name="price">Price of trade</param>
        /// <param name="side">Side of trade</param>
        /// <returns>OrderResponse object</returns>
        public OrderResponse LimitOrder(string pair, decimal quantity, decimal price, Side side)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.LimitOrder(pair, quantity, price, side);
        }

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <param name="price">Price of trade</param>
        /// <param name="side">Side of trade</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderResponse> LimitOrderAsync(string pair, decimal quantity, decimal price, Side side)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.LimitOrderAsync(pair, quantity, price, side);
        }

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <param name="side">Side of trade</param>
        /// <returns>OrderResponse object</returns>
        public OrderResponse MarketOrder(string pair, decimal quantity, Side side)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.MarketOrder(pair, quantity, side);
        }

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <param name="side">Side of trade</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderResponse> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.MarketOrderAsync(pair, quantity, side);
        }

        /// <summary>
        /// Place a stop loss order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <param name="price">Price of order</param>
        /// <param name="stopPrice">Price of Stop Loss</param>
        /// <param name="side">Side of trade</param>
        /// <returns>OrderResponse object</returns>
        public OrderResponse StopLossOrder(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.StopLossOrder(pair, quantity, price, stopPrice, side);
        }

        /// <summary>
        /// Place a stop loss order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <param name="price">Price of order</param>
        /// <param name="stopPrice">Price of Stop Loss</param>
        /// <param name="side">Side of trade</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderResponse> StopLossOrderAsync(string pair, decimal quantity, decimal price, decimal stopPrice, Side side)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.StopLossOrderAsync(pair, quantity, price, stopPrice, side);
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="pair">Trading pair of order</param>
        /// <returns>OrderResponse object</returns>
        public OrderResponse CancelOrder(string orderId, string pair)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.CancelOrder(orderId, pair);
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="pair">Trading pair of order</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderResponse> CancelOrderAsync(string orderId, string pair)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.CancelOrderAsync(orderId, pair);
        }

        /// <summary>
        /// Get KLines for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="interval">Time interval</param>
        /// <param name="limit">Number of klines to return</param>
        /// <returns>Collection of KLines</returns>
        public KLine[] GetKLines(string pair, TimeInterval interval, int limit = 20)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.GetKLines(pair, interval, limit);
        }

        /// <summary>
        /// Get KLines for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="interval">Time interval</param>
        /// <param name="limit">Number of klines to return</param>
        /// <returns>Collection of KLines</returns>
        public async Task<KLine[]> GetKLinesAsync(string pair, TimeInterval interval, int limit = 20)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.GetKLinesAsync(pair, interval, limit);
        }

        /// <summary>
        /// Get 24 hour stats for a trading pair
        /// </summary>
        /// <param name="pair">Trading Pair</param>
        /// <returns>Ticker object</returns>
        public Ticker Get24hrStats(string pair)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.Get24hrStats(pair);
        }

        /// <summary>
        /// Get 24 hour stats for a trading pair
        /// </summary>
        /// <param name="pair">Trading Pair</param>
        /// <returns>Ticker object</returns>
        public async Task<Ticker> Get24hrStatsAsync(string pair)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.Get24hrStatsAsync(pair);
        }

        /// <summary>
        /// Get deposit address for a currency
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Dictionary of address</returns>
        public Dictionary<string, string> GetDepositAddress(string symbol)
        {
            return proxy.GetDepositAddress(symbol);
        }

        /// <summary>
        /// Get deposit address for a currency
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Dictionary of address</returns>
        public async Task<Dictionary<string, string>> GetDepositAddressAsync(string symbol)
        {
            return await proxy.GetDepositAddressAsync(symbol);
        }

        /// <summary>
        /// Get order book for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="limit">Number of orders to return</param>
        /// <returns>Orderbook object</returns>
        public OrderBook GetOrderBook(string pair, int limit = 100)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.GetOrderBook(pair, limit);
        }

        /// <summary>
        /// Get order book for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="limit">Number of orders to return</param>
        /// <returns>Orderbook object</returns>
        public async Task<OrderBook> GetOrderBookAsync(string pair, int limit = 100)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.GetOrderBookAsync(pair, limit);
        }

        /// <summary>
        /// Get an order
        /// </summary>
        /// <param name="pair">Trading pair of order</param>
        /// <param name="orderId">Order id</param>
        /// <returns>OrderResponse object</returns>
        public OrderResponse GetOrder(string pair, string orderId)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.GetOrder(pair, orderId);
        }

        /// <summary>
        /// Get an order
        /// </summary>
        /// <param name="pair">Trading pair of order</param>
        /// <param name="orderId">Order id</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderResponse> GetOrderAsync(string pair, string orderId)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.GetOrderAsync(pair, orderId);
        }

        /// <summary>
        /// Get account orders for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="limit">Number of orders to return</param>
        /// <returns>Collection of OrderResponse objects</returns>
        public IEnumerable<OrderResponse> GetOrders(string pair, int limit = 20)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.GetOrders(pair, limit);
        }

        /// <summary>
        /// Get account orders for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="limit">Number of orders to return</param>
        /// <returns>Collection of OrderResponse objects</returns>
        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(string pair, int limit = 20)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.GetOrdersAsync(pair, limit);
        }

        /// <summary>
        /// Get account open orders for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Collection of OrderResponse objects</returns>
        public IEnumerable<OrderResponse> GetOpenOrders(string pair)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return proxy.GetOpenOrders(pair);
        }

        /// <summary>
        /// Get account open orders for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Collection of OrderResponse objects</returns>
        public async Task<IEnumerable<OrderResponse>> GetOpenOrdersAsync(string pair)
        {
            pair = proxy.FormatPair(pair, Exchange.KuCoin);

            return await proxy.GetOpenOrdersAsync(pair);
        }
    }
}
