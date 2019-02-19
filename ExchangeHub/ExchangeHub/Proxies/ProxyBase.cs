using DateTimeHelpers;
using ExchangeHub.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeHub.Proxies
{
    public class ProxyBase
    {
        private DateTimeHelper _dtHelper;
        private Dictionary<string, string> _pairs;

        public ProxyBase()
        {
            this._dtHelper = new DateTimeHelper();
            this._pairs = new Dictionary<string, string>();
        }

        /// <summary>
        /// Set trading pairs for exchange
        /// </summary>
        /// <param name="pairs">Pairs to set</param>
        public void SetPairs(string[] pairs)
        {
            for (var i = 0; i < pairs.Length; i++)
            {
                _pairs.Add(pairs[i].Replace("-", ""), pairs[i]);
            }
        }

        /// <summary>
        /// Get trading pairs for an exchange
        /// </summary>
        /// <returns>Dictionary of trading pairs</returns>
        public Dictionary<string, string> GetPairs()
        {
            return _pairs;
        }

        /// <summary>
        /// Get formatted trading pair for an exchange
        /// </summary>
        /// <param name="pair">Pair to return</param>
        /// <param name="exchange">Exchange to format</param>
        /// <returns>String of trading pair</returns>
        public string FormatPair(string pair, Contracts.Exchange exchange)
        {
            if (string.IsNullOrEmpty(pair))
                return string.Empty;

            else if(exchange == Contracts.Exchange.Binance)
                return pair.Replace("-", "");
            
            else if (pair.IndexOf("-") > 0)
                return pair;

            return _pairs[pair];
        }

        public long IntervalToMS(TimeInterval interval)
        {
            var ms = 0;
            var second = 1000;
            var minute = second * 60;
            var hour = minute * 60;
            var day = hour * 24;

            switch(interval)
            {
                case TimeInterval.EightH:
                    ms = hour * 8;
                    break;
                case TimeInterval.FifteenM:
                    ms = minute * 15;
                    break;
                case TimeInterval.FiveM:
                    ms = minute * 5;
                    break;
                case TimeInterval.FourH:
                    ms = hour * 4;
                    break;
                case TimeInterval.None:
                    break;
                case TimeInterval.OneD:
                    ms = day;
                    break;
                case TimeInterval.OneH:
                    ms = hour;
                    break;
                case TimeInterval.OneM:
                    ms = minute;
                    break;
                case TimeInterval.OneMo:
                    ms = day * 30;
                    break;
                case TimeInterval.OneW:
                    ms = day * 7;
                    break;
                case TimeInterval.SixH:
                    ms = hour * 6;
                    break;
                case TimeInterval.ThirtyM:
                    ms = minute * 30;
                    break;
                case TimeInterval.ThreeD:
                    ms = day * 3;
                    break;
                case TimeInterval.ThreeM:
                    ms = minute * 3;
                    break;
                case TimeInterval.TwelveH:
                    ms = hour * 12;
                    break;
                case TimeInterval.TwoH:
                    ms = hour * 2;
                    break;
                default:
                    break;
            }

            return ms;
        }

        #region Binance

        public IEnumerable<Contracts.PairPrice> BinanceTickerCollectionConverter(Binance.NetCore.Entities.Ticker[] tickers)
        {

            var pairPriceList = new List<Contracts.PairPrice>();

            for(var i = 0; i < tickers.Length; i++)
            {
                var pairPrice = BinanceTickerConverter(tickers[i]);

                pairPriceList.Add(pairPrice);
            }

            return pairPriceList;
        }

        public Contracts.PairPrice BinanceTickerConverter(Binance.NetCore.Entities.Ticker ticker)
        {
            var pairPrice = new Contracts.PairPrice
            {
                Pair = ticker.Symbol,
                Price = ticker.Price
            };

            return pairPrice;
        }

        public Contracts.Balance BinanceBalanceConverter(Binance.NetCore.Entities.Balance binanceBal)
        {
            var balance = new Contracts.Balance
            {
                Available = binanceBal.free,
                Frozen = binanceBal.locked,
                Symbol = binanceBal.asset
            };

            return balance;
        }

        public Contracts.OrderResponse BinanceTradeResponseConverter(Binance.NetCore.Entities.TradeResponse tradeResp)
        {
            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = tradeResp.executedQty,
                OrderId = tradeResp.orderId.ToString(),
                OrderQuantity = tradeResp.orderId,
                OrderStatus = BinanceOrderStatusConverter(tradeResp.status),
                Pair = tradeResp.symbol,
                Price = tradeResp.price,
                Side = BinanceSideConverter(tradeResp.side),
                TransactTime = _dtHelper.UnixTimeToUTC(tradeResp.transactTime)
            };

            return orderResponse;
        }

        public Contracts.OrderBook BinanceOrderBookConverter(Binance.NetCore.Entities.OrderBook binanceBook)
        {
            var orderBook = new Contracts.OrderBook
            {
                asks = BinanceOrderCollectionConverter(binanceBook.asks),
                bids = BinanceOrderCollectionConverter(binanceBook.bids)
            };

            return orderBook;
        }

        public Contracts.Order[] BinanceOrderCollectionConverter(Binance.NetCore.Entities.Orders[] binanceOrders)
        {
            var orders = new List<Contracts.Order>();

            for(var i = 0;i< binanceOrders.Length;i++)
            {
                var order = BinanceOrderConverter(binanceOrders[i]);

                orders.Add(order);
            }

            return orders.ToArray();
        }

        public Contracts.Order BinanceOrderConverter(Binance.NetCore.Entities.Orders binanceOrder)
        {
            var order = new Contracts.Order
            {
                price = binanceOrder.price,
                quantity = binanceOrder.quantity
            };

            return order;
        }

        public Contracts.OrderResponse BinanceOrderResponseToOrderResponse(Binance.NetCore.Entities.OrderResponse binanceResponse)
        {
            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = binanceResponse.executedQty,
                OrderId = binanceResponse.orderId.ToString(),
                OrderQuantity = binanceResponse.origQty,
                OrderStatus = BinanceOrderStatusConverter(binanceResponse.status),
                Price = binanceResponse.price,
                Side = BinanceSideConverter(binanceResponse.side),
                StopPrice = binanceResponse.stopPrice,
                Pair = binanceResponse.symbol,
                TransactTime = _dtHelper.UnixTimeToUTC(binanceResponse.time)
            };

            return orderResponse;
        }

        public Contracts.OrderResponse BinanceTradeResponseToOrderResponse(Binance.NetCore.Entities.TradeResponse tradeResponse)
        {
            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = tradeResponse.executedQty,
                OrderId = tradeResponse.orderId.ToString(),
                OrderQuantity = tradeResponse.origQty,
                OrderStatus = BinanceOrderStatusConverter(tradeResponse.status),
                Price = tradeResponse.price,
                Side = BinanceSideConverter(tradeResponse.side),
                Pair = tradeResponse.symbol,
                TransactTime = _dtHelper.UnixTimeToUTC(tradeResponse.transactTime)
            };

            return orderResponse;
        }

        public Contracts.KLine BinanceCandlestickToKLine(Binance.NetCore.Entities.Candlestick candlestick)
        {
            var kline = new Contracts.KLine
            {
                Close = candlestick.close,
                CloseTime = _dtHelper.UnixTimeToUTC(candlestick.closeTime),
                High = candlestick.high,
                Low = candlestick.low,
                Open = candlestick.open,
                OpenTime = _dtHelper.UnixTimeToUTC(candlestick.openTime),
                Volume = candlestick.volume
            };

            return kline;
        }

        public Contracts.Ticker BinanceTickToTicker(Binance.NetCore.Entities.Tick tick)
        {
            var ticker = new Contracts.Ticker
            {
                AskPrice = decimal.Parse(tick.askPrice),
                AskQty = decimal.Parse(tick.askQty),
                BidPrice = decimal.Parse(tick.bidPrice),
                BidQty = decimal.Parse(tick.bidQty),
                CloseTime = _dtHelper.UnixTimeToUTC((long)tick.closeTime),
                High = decimal.Parse(tick.highPrice),
                LastPrice = decimal.Parse(tick.lastPrice),
                LastQty = decimal.Parse(tick.lastQty),
                Low = decimal.Parse(tick.lowPrice),
                Open = decimal.Parse(tick.openPrice),
                OpenTime = _dtHelper.UnixTimeToUTC((long)tick.openTime),
                PreviousClosePrice = decimal.Parse(tick.prevClosePrice),
                PriceChange = decimal.Parse(tick.priceChange),
                PriceChangePercent = decimal.Parse(tick.priceChangePercent),
                Pair = tick.symbol,
                Volume = decimal.Parse(tick.volume),
                WeightedAvgPrice = decimal.Parse(tick.weightedAvgPrice),
            };

            return ticker;
        }

        public Contracts.OrderStatus BinanceOrderStatusConverter(Binance.NetCore.Entities.OrderStatus binanceStatus)
        {
            Contracts.OrderStatus orderStatus;

            switch (binanceStatus)
            {
                case Binance.NetCore.Entities.OrderStatus.CANCELED:
                    orderStatus = Contracts.OrderStatus.Canceled;
                    break;
                case Binance.NetCore.Entities.OrderStatus.EXPIRED:
                    orderStatus = Contracts.OrderStatus.Canceled;
                    break;
                case Binance.NetCore.Entities.OrderStatus.FILLED:
                    orderStatus = Contracts.OrderStatus.Filled;
                    break;
                case Binance.NetCore.Entities.OrderStatus.NEW:
                    orderStatus = Contracts.OrderStatus.Open;
                    break;
                case Binance.NetCore.Entities.OrderStatus.PARTIALLY_FILLED:
                    orderStatus = Contracts.OrderStatus.PartialFill;
                    break;
                case Binance.NetCore.Entities.OrderStatus.REJECTED:
                    orderStatus = Contracts.OrderStatus.Canceled;
                    break;
                default:
                    orderStatus = Contracts.OrderStatus.Open;
                    break;
            }

            return orderStatus;
        }

        public Contracts.TimeInterval BinanceIntervalConverter(Binance.NetCore.Entities.Interval binanceInterval)
        {
            Contracts.TimeInterval interval;

            switch (binanceInterval)
            {
                case Binance.NetCore.Entities.Interval.EightH:
                    interval = Contracts.TimeInterval.EightH;
                    break;
                case Binance.NetCore.Entities.Interval.FifteenM:
                    interval = Contracts.TimeInterval.FifteenM;
                    break;
                case Binance.NetCore.Entities.Interval.FiveM:
                    interval = Contracts.TimeInterval.FiveM;
                    break;
                case Binance.NetCore.Entities.Interval.FourH:
                    interval = Contracts.TimeInterval.FourH;
                    break;
                case Binance.NetCore.Entities.Interval.None:
                    interval = Contracts.TimeInterval.None;
                    break;
                case Binance.NetCore.Entities.Interval.OneD:
                    interval = Contracts.TimeInterval.OneD;
                    break;
                case Binance.NetCore.Entities.Interval.OneH:
                    interval = Contracts.TimeInterval.OneH;
                    break;
                case Binance.NetCore.Entities.Interval.OneM:
                    interval = Contracts.TimeInterval.OneM;
                    break;
                case Binance.NetCore.Entities.Interval.OneMo:
                    interval = Contracts.TimeInterval.OneMo;
                    break;
                case Binance.NetCore.Entities.Interval.OneW:
                    interval = Contracts.TimeInterval.OneW;
                    break;
                case Binance.NetCore.Entities.Interval.SixH:
                    interval = Contracts.TimeInterval.SixH;
                    break;
                case Binance.NetCore.Entities.Interval.ThirtyM:
                    interval = Contracts.TimeInterval.ThirtyM;
                    break;
                case Binance.NetCore.Entities.Interval.ThredD:
                    interval = Contracts.TimeInterval.ThreeD;
                    break;
                case Binance.NetCore.Entities.Interval.ThreeM:
                    interval = Contracts.TimeInterval.ThreeM;
                    break;
                case Binance.NetCore.Entities.Interval.TwelveH:
                    interval = Contracts.TimeInterval.TwelveH;
                    break;
                case Binance.NetCore.Entities.Interval.TwoH:
                    interval = Contracts.TimeInterval.TwoH;
                    break;
                default:
                    interval = Contracts.TimeInterval.OneH;
                    break;
            }

            return interval;
        }


        public Binance.NetCore.Entities.Interval BinanceIntervalReConverter(Contracts.TimeInterval timeInterval)
        {
            Binance.NetCore.Entities.Interval binanceInterval;

            switch (timeInterval)
            {
                case Contracts.TimeInterval.EightH:
                    binanceInterval = Binance.NetCore.Entities.Interval.EightH;
                    break;
                case Contracts.TimeInterval.FifteenM:
                    binanceInterval = Binance.NetCore.Entities.Interval.FifteenM;
                    break;
                case Contracts.TimeInterval.FiveM:
                    binanceInterval = Binance.NetCore.Entities.Interval.FiveM;
                    break;
                case Contracts.TimeInterval.FourH:
                    binanceInterval = Binance.NetCore.Entities.Interval.FourH;
                    break;
                case Contracts.TimeInterval.None:
                    binanceInterval = Binance.NetCore.Entities.Interval.None;
                    break;
                case Contracts.TimeInterval.OneD:
                    binanceInterval = Binance.NetCore.Entities.Interval.OneD;
                    break;
                case Contracts.TimeInterval.OneH:
                    binanceInterval = Binance.NetCore.Entities.Interval.OneH;
                    break;
                case Contracts.TimeInterval.OneM:
                    binanceInterval = Binance.NetCore.Entities.Interval.OneM;
                    break;
                case Contracts.TimeInterval.OneMo:
                    binanceInterval = Binance.NetCore.Entities.Interval.OneMo;
                    break;
                case Contracts.TimeInterval.OneW:
                    binanceInterval = Binance.NetCore.Entities.Interval.OneW;
                    break;
                case Contracts.TimeInterval.SixH:
                    binanceInterval = Binance.NetCore.Entities.Interval.SixH;
                    break;
                case Contracts.TimeInterval.ThirtyM:
                    binanceInterval = Binance.NetCore.Entities.Interval.ThirtyM;
                    break;
                case Contracts.TimeInterval.ThreeD:
                    binanceInterval = Binance.NetCore.Entities.Interval.ThredD;
                    break;
                case Contracts.TimeInterval.ThreeM:
                    binanceInterval = Binance.NetCore.Entities.Interval.ThreeM;
                    break;
                case Contracts.TimeInterval.TwelveH:
                    binanceInterval = Binance.NetCore.Entities.Interval.TwelveH;
                    break;
                case Contracts.TimeInterval.TwoH:
                    binanceInterval = Binance.NetCore.Entities.Interval.TwoH;
                    break;
                default:
                    binanceInterval = Binance.NetCore.Entities.Interval.OneH;
                    break;
            }

            return binanceInterval;
        }

        public Contracts.Side BinanceSideConverter(Binance.NetCore.Entities.Side binanceSide)
        {
            Contracts.Side side;

            switch (binanceSide)
            {
                case Binance.NetCore.Entities.Side.BUY:
                    side = Contracts.Side.Buy;
                    break;
                case Binance.NetCore.Entities.Side.SELL:
                    side = Contracts.Side.Sell;
                    break;
                default:
                    side = Contracts.Side.Buy;
                    break;
            }

            return side;
        }

        public Binance.NetCore.Entities.Side BinanceSideReConverter(Contracts.Side side)
        {
            Binance.NetCore.Entities.Side binanceSide;

            switch (side)
            {
                case Contracts.Side.Buy:
                    binanceSide = Binance.NetCore.Entities.Side.BUY;
                    break;
                case Contracts.Side.Sell:
                    binanceSide = Binance.NetCore.Entities.Side.SELL;
                    break;
                default:
                    binanceSide = Binance.NetCore.Entities.Side.BUY;
                    break;
            }

            return binanceSide;
        }


        public Contracts.Side BinanceTradeTypeConverter(Binance.NetCore.Entities.TradeType tradeType)
        {
            Contracts.Side side;

            switch (tradeType)
            {
                case Binance.NetCore.Entities.TradeType.BUY:
                    side = Contracts.Side.Buy;
                    break;
                case Binance.NetCore.Entities.TradeType.SELL:
                    side = Contracts.Side.Sell;
                    break;
                default:
                    side = Contracts.Side.Buy;
                    break;
            }

            return side;
        }

        #endregion Binance

        #region Bittrex

        public Contracts.Balance BittrexBalanceConverter(BittrexApi.NetCore.Entities.Balance bittrexBalance)
        {
            var balance = new Contracts.Balance
            {
                Available = bittrexBalance.available,
                Frozen = bittrexBalance.pending,
                Symbol = bittrexBalance.symbol
            };

            return balance;
        }

        public Contracts.OrderBook BittrexOrderBookConverter(BittrexApi.NetCore.Entities.OrderBook bittrexBook)
        {
            var orderBook = new Contracts.OrderBook
            {
                asks = BittrexOrderIntervalCollectionConverter(bittrexBook.sell),
                bids = BittrexOrderIntervalCollectionConverter(bittrexBook.buy)
            };

            return orderBook;
        }

        public Contracts.Order[] BittrexOrderIntervalCollectionConverter(BittrexApi.NetCore.Entities.OrderInterval[] bittrexOrders)
        {
            var orders = new List<Contracts.Order>();

            for(var i = 0; i< bittrexOrders.Length;i++)
            {
                var order = BittrexOrderIntervalConverter(bittrexOrders[i]);

                orders.Add(order);
            }

            return orders.ToArray();
        }

        public Contracts.Order BittrexOrderIntervalConverter(BittrexApi.NetCore.Entities.OrderInterval bittrexOrder)
        {
            var order = new Contracts.Order
            {
                price = bittrexOrder.rate,
                quantity = bittrexOrder.quantity
            };

            return order;
        }

        public Contracts.Ticker BittrexMarketSummaryConverter(BittrexApi.NetCore.Entities.MarketSummary marketSummary)
        {
            var ticker = new Contracts.Ticker
            {
                AskPrice = marketSummary.ask,
                AskQty = marketSummary.openSells,
                BidPrice = marketSummary.bid,
                BidQty = marketSummary.openBuys,
                High = marketSummary.high,
                LastPrice = marketSummary.last,
                Low = marketSummary.low,
                Open = marketSummary.prevDay,
                OpenTime = marketSummary.created,
                Pair = marketSummary.marketName,
                PriceChange = marketSummary.last - marketSummary.prevDay,
                PriceChangePercent = 1 - (marketSummary.prevDay / marketSummary.last),
                Volume = marketSummary.volume                
            };

            return ticker;
        }

        public IEnumerable<Contracts.PairPrice> BittrexMarketSummaryCollectionToPairPriceConverter(BittrexApi.NetCore.Entities.MarketSummary[] marketSummaries)
        {
            var pairPriceList = new List<Contracts.PairPrice>();

            for (var i = 0; i < marketSummaries.Length; i++)
            {
                var pairPrice = BittrexMarketSummaryToPairPriceConverter(marketSummaries[i]);
                pairPriceList.Add(pairPrice);
            }

            return pairPriceList;
        }

        public Contracts.PairPrice BittrexMarketSummaryToPairPriceConverter(BittrexApi.NetCore.Entities.MarketSummary marketSummary)
        {
            var pairPrice = new Contracts.PairPrice
            {
                Pair = marketSummary.pair,                
                Price = marketSummary.last
            };

            return pairPrice;
        }

        public Contracts.OrderResponse BittrexOrderToOrderResponse(BittrexApi.NetCore.Entities.Order bittrexOrder)
        {
            Contracts.OrderStatus orderStatus;

            if (bittrexOrder.quantity == bittrexOrder.quantityRemaining)
            {
                orderStatus = Contracts.OrderStatus.Open;
            }
            else if (bittrexOrder.quantityRemaining > 0)
            {
                orderStatus = Contracts.OrderStatus.PartialFill;
            }
            else
            {
                orderStatus = Contracts.OrderStatus.Filled;
            }

            Contracts.Side side = bittrexOrder.orderType.Equals("LIMIT_BUY")
                ? Contracts.Side.Buy : Contracts.Side.Sell;

            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = bittrexOrder.quantity - bittrexOrder.quantityRemaining,
                OrderId = bittrexOrder.orderId,
                OrderQuantity = bittrexOrder.quantity,
                OrderStatus = orderStatus,
                Price = bittrexOrder.price,
                Side = side,
                Pair = bittrexOrder.pair
            };

            return orderResponse;
        }

        public Contracts.OrderResponse BittrexOrderDetailToOrderResponse(BittrexApi.NetCore.Entities.OrderDetail tradeResponse)
        {
            Contracts.OrderStatus orderStatus;

            if (tradeResponse.cancelInitiated)
            {
                orderStatus = Contracts.OrderStatus.Canceled;
            }
            else
            {
                if (tradeResponse.quantity == tradeResponse.quantityRemaining)
                {
                    orderStatus = Contracts.OrderStatus.Open;
                }
                else if (tradeResponse.quantityRemaining > 0)
                {
                    orderStatus = Contracts.OrderStatus.PartialFill;
                }
                else
                {
                    orderStatus = Contracts.OrderStatus.Filled;
                }
            }

            Contracts.Side side = tradeResponse.orderType.Equals("LIMIT_BUY")
                ? Contracts.Side.Buy : Contracts.Side.Sell;

            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = tradeResponse.quantity - tradeResponse.quantityRemaining,
                OrderId = tradeResponse.orderId,
                OrderQuantity = tradeResponse.quantity,
                OrderStatus = orderStatus,
                Price = tradeResponse.price,
                Side = side,
                Pair = tradeResponse.pair,
                TransactTime = tradeResponse.opened
            };

            return orderResponse;
        }

        public Contracts.OrderResponse BittrexOpenOrderToOrderResponse(BittrexApi.NetCore.Entities.OpenOrder openOrder)
        {
            Contracts.OrderStatus orderStatus;

            if (openOrder.cancelInitiated)
            {
                orderStatus = Contracts.OrderStatus.Canceled;
            }
            else
            {
                if (openOrder.quantity == openOrder.quantityRemaining)
                {
                    orderStatus = Contracts.OrderStatus.Open;
                }
                else if (openOrder.quantityRemaining > 0)
                {
                    orderStatus = Contracts.OrderStatus.PartialFill;
                }
                else
                {
                    orderStatus = Contracts.OrderStatus.Filled;
                }
            }

            Contracts.Side side = openOrder.orderType.Equals("LIMIT_BUY")
                ? Contracts.Side.Buy : Contracts.Side.Sell;

            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = openOrder.quantity - openOrder.quantityRemaining,
                OrderId = openOrder.orderId,
                OrderQuantity = openOrder.quantity,
                OrderStatus = orderStatus,
                Price = openOrder.price,
                Side = side,
                Pair = openOrder.pair,
                TransactTime = openOrder.opened
            };

            return orderResponse;
        }

        public Contracts.Side BittrexSideConverter(BittrexApi.NetCore.Entities.Side bittrexSide)
        {
            Contracts.Side side;

            switch (bittrexSide)
            {
                case BittrexApi.NetCore.Entities.Side.BUY:
                    side = Contracts.Side.Buy;
                    break;
                case BittrexApi.NetCore.Entities.Side.SELL:
                    side = Contracts.Side.Sell;
                    break;
                default:
                    side = Contracts.Side.Buy;
                    break;
            }

            return side;
        }

        public BittrexApi.NetCore.Entities.Side BittrexSideReConverter(Contracts.Side side)
        {
            BittrexApi.NetCore.Entities.Side bittrexSide;

            switch (side)
            {
                case Contracts.Side.Buy:
                    bittrexSide = BittrexApi.NetCore.Entities.Side.BUY;
                    break;
                case Contracts.Side.Sell:
                    bittrexSide = BittrexApi.NetCore.Entities.Side.SELL;
                    break;
                default:
                    bittrexSide = BittrexApi.NetCore.Entities.Side.BUY;
                    break;
            }

            return bittrexSide;
        }
        #endregion Bittrex

        #region CoinbasePro

        public Contracts.PairPrice CoinbaseProTickerConverter(CoinbaseProApi.NetCore.Entities.Ticker ticker, string pair)
        {
            var pairPrice = new Contracts.PairPrice
            {
                Pair = pair,
                Price = ticker.price
            };
            
            return pairPrice;
        }

        public Contracts.PairPrice CoinbaseProFillConverter(CoinbaseProApi.NetCore.Entities.Fill fill)
        {
            var pairPrice = new Contracts.PairPrice
            {
                Pair = fill.product_id,
                Price = fill.price
            };

            return pairPrice;
        }

        public Contracts.Balance[] CoinbaseProAccountCollectionConverter(CoinbaseProApi.NetCore.Entities.Account[] accounts)
        {
            var balances = new List<Contracts.Balance>();

            foreach(var account in accounts)
            {
                var balance = CoinbaseProAccountConverter(account);

                balances.Add(balance);
            }

            return balances.ToArray();
        }

        public Contracts.Balance CoinbaseProAccountConverter(CoinbaseProApi.NetCore.Entities.Account account)
        {
            var balance = new Contracts.Balance
            {
                Available = account.available,
                Frozen = account.hold,
                Symbol = account.currency
            };

            return balance;
        }

        public Contracts.OrderResponse[] CoinbaseProOrderResponseCollectionConverter(CoinbaseProApi.NetCore.Entities.OrderResponse[] cbResponse)
        {
            var orderResponses = new List<Contracts.OrderResponse>();

            for (var i = 0; i < cbResponse.Length; i++)
            {
                var orderResponse = CoinbaseProOrderResponseConverter(cbResponse[i]);
                orderResponses.Add(orderResponse);
            }

            return orderResponses.ToArray();
        }

        public Contracts.OrderResponse CoinbaseProOrderResponseConverter(CoinbaseProApi.NetCore.Entities.OrderResponse cbResponse)
        {
            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = cbResponse.filled_size,
                OrderId = cbResponse.id,
                OrderQuantity = cbResponse.filled_size,
                OrderStatus = CoinbaseProOrderStatusConverter(cbResponse.status),
                Pair = cbResponse.product_id,
                Price = cbResponse.price,
                Side = CoinbaseProSideConverter(cbResponse.side),
                TransactTime = cbResponse.created_at.DateTime
            };

            return orderResponse;
        }

        public Contracts.Ticker CoinbaseProStatsConverter(CoinbaseProApi.NetCore.Entities.PairStats stats)
        {
            var ticker = new Contracts.Ticker
            {
                High = stats.high,
                Low = stats.low,
                Open = stats.open,
                Volume = stats.volume
            };

            return ticker;
        }

        public Contracts.OrderBook CoinbaseProOrderBookResponseConverter(CoinbaseProApi.NetCore.Entities.OrderBookResponse obr)
        {
            var orderBook = new Contracts.OrderBook
            {
                asks = CoinbaseProOrderBookCollectionConverter(obr.sells),
                bids = CoinbaseProOrderBookCollectionConverter(obr.buys)
            };

            return orderBook;
        }

        public Contracts.Order[] CoinbaseProOrderBookCollectionConverter(CoinbaseProApi.NetCore.Entities.OrderBook[] orderBook)
        {
            var orders = new List<Contracts.Order>();

            for (var i = 0; i < orderBook.Length; i++)
            {
                var order = CoinbaseProOrderBookConverter(orderBook[i]);
                orders.Add(order);
            }

            return orders.ToArray();
        }

        public Contracts.Order CoinbaseProOrderBookConverter(CoinbaseProApi.NetCore.Entities.OrderBook orderBook)
        {
            var order = new Contracts.Order
            {
                price = orderBook.price,
                quantity = orderBook.size
            };

            return order;
        }

        public Contracts.OrderResponse CoinbaseProOrderConverter(CoinbaseProApi.NetCore.Entities.Order order)
        {
            var orderResonse = new Contracts.OrderResponse
            {
                FilledQuantity = order.fill_size,
                OrderId = order.id,
                OrderQuantity = order.size,
                OrderStatus = CoinbaseProOrderStatusConverter(order.status),
                Pair = order.product_id,
                Price = order.executed_value,
                Side = CoinbaseProSideConverter(order.side),
                TransactTime = order.crated_at.DateTime
            };

            return orderResonse;
        }

        public Contracts.OrderStatus CoinbaseProOrderStatusConverter(string status)
        {
            Contracts.OrderStatus orderStatus;

            switch(status)
            {
                case "open":
                    orderStatus = Contracts.OrderStatus.Open;
                    break;
                case "closed":
                    orderStatus = Contracts.OrderStatus.Filled;
                    break;
                case "active":
                    orderStatus = Contracts.OrderStatus.PartialFill;
                    break;
                default:
                    orderStatus = Contracts.OrderStatus.Open;
                    break;
            }

            return orderStatus;
        }

        public Contracts.Side CoinbaseProSideConverter(string cbpSide)
        {
            Contracts.Side side;

            switch (cbpSide.ToLower())
            {
                case "buy":
                    side = Contracts.Side.Buy;
                    break;
                case "sell":
                    side = Contracts.Side.Sell;
                    break;
                default:
                    side = Contracts.Side.Buy;
                    break;
            }

            return side;
        }

        public CoinbaseProApi.NetCore.Entities.SIDE CoinbaseProSideReConverter(Contracts.Side side)
        {
            CoinbaseProApi.NetCore.Entities.SIDE coinbaseSide;

            switch (side)
            {
                case Contracts.Side.Buy:
                    coinbaseSide = CoinbaseProApi.NetCore.Entities.SIDE.BUY;
                    break;
                case Contracts.Side.Sell:
                    coinbaseSide = CoinbaseProApi.NetCore.Entities.SIDE.SELL;
                    break;
                default:
                    coinbaseSide = CoinbaseProApi.NetCore.Entities.SIDE.BUY;
                    break;
            }

            return coinbaseSide;
        }

        #endregion CoinbasePro

        #region KuCoin

        public Contracts.OrderBook KuCoinOrderBookConverter(KuCoinApi.Net.Entities.OrderBookL2 kuOrderBook)
        {
            var orderBook = new Contracts.OrderBook
            {
                asks = KuCoinOrderBookCollectionConverter(kuOrderBook.Asks),
                bids = KuCoinOrderBookCollectionConverter(kuOrderBook.Bids)
            };

            return orderBook;
        }

        public Contracts.Order[] KuCoinOrderBookCollectionConverter(KuCoinApi.Net.Entities.OrderBookDetailL2[] orderBooks)
        {
            var orders = new List<Contracts.Order>();

            for(var i =0; i< orderBooks.Length; i++)
            {
                var order = new Order
                {
                    price = orderBooks[i].Price,
                    quantity = orderBooks[i].Size
                };
                orders.Add(order);
            }

            return orders.ToArray();
        }

        public Contracts.Balance[] KuCoinBalanceCollectionConverter(List<KuCoinApi.Net.Entities.Balance> kuBalances)
        {
            var balances = new List<Contracts.Balance>();

            for(var i = 0; i < kuBalances.Count; i++)
            {
                var balance = KuCoinBalanceConverter(kuBalances[i]);
                balances.Add(balance);
            }

            return balances.ToArray();
        }

        public Contracts.Balance KuCoinBalanceConverter(KuCoinApi.Net.Entities.Balance kuBalance)
        {
            var balance = new Contracts.Balance
            {
                Available = kuBalance.Available,
                Frozen = kuBalance.Frozen,
                Symbol = kuBalance.Symbol
            };

            return balance;
        }
        
        public Contracts.PairPrice KuCoinStatsToPairPrice(KuCoinApi.Net.Entities.TradingPairStats stats)
        {
            var pairPrice = new Contracts.PairPrice
            {
                Pair = stats.Pair,
                Price = stats.Close
            };

            return pairPrice;
        }

        public IEnumerable<Contracts.PairPrice> KuCoinStatsCollectionToPairPrice(List<KuCoinApi.Net.Entities.TradingPairStats> stats)
        {
            var pairPriceList = new List<Contracts.PairPrice>();

            for (var i = 0; i < stats.Count; i++)
            {
                var pairPrice = KuCoinStatsToPairPrice(stats[i]);

                pairPriceList.Add(pairPrice);
            }

            return pairPriceList;
        }

        public Contracts.Ticker KuCoinTickToTicker(KuCoinApi.Net.Entities.Ticker tick, KuCoinApi.Net.Entities.TradingPairStats stats)
        {
            var ticker = new Contracts.Ticker
            {
                AskPrice = tick.BestAsk,
                BidPrice = tick.BestBid,
                High = stats.High,
                LastPrice = tick.Price,
                Low = stats.Low,
                Pair = stats.Pair,
                PriceChangePercent = stats.ChangeRate,
                Volume = stats.Volume
            };

            return ticker;
        }

        public Contracts.OrderResponse KuCoinOrderConverter(KuCoinApi.Net.Entities.Order order)
        {
            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = order.DealSize,
                OrderId = order.Id,
                OrderQuantity = order.Size,
                Price = order.Price,
                Side = order.Side.Equals("BUY") ? Contracts.Side.Buy : Contracts.Side.Sell,
                Pair = order.Pair,
                TransactTime = _dtHelper.UnixTimeToUTC(order.CreatedAt)
            };

            return orderResponse;
        }
        
        public Contracts.KLine[] KuCoinCandlesticksConverter(List<KuCoinApi.Net.Entities.Candlestick> candlesticks, TimeInterval interval)
        {
            var klineList = new List<Contracts.KLine>();
            var timeDiff = this.IntervalToMS(interval);

            for(int i = 0; i < candlesticks.Count; i++)
            {
                var kline = new Contracts.KLine
                {
                    Close = candlesticks[i].Close,
                    CloseTime = _dtHelper.UnixTimeToUTC((candlesticks[i].StartTime + timeDiff)),
                    High = candlesticks[i].High,
                    Low = candlesticks[i].Low,
                    Open = candlesticks[i].Open,
                    Volume = candlesticks[i].Volume
                };

                klineList.Add(kline);
            }

            return klineList.ToArray();
        }

        public KuCoinApi.NetCore.Entities.TradeType KuCoinTradeTypeReConverter(Contracts.Side side)
        {
            KuCoinApi.NetCore.Entities.TradeType tradeType = KuCoinApi.NetCore.Entities.TradeType.NONE;

            if (side == Contracts.Side.Buy)
                tradeType = KuCoinApi.NetCore.Entities.TradeType.BUY;
            else if (side == Contracts.Side.Sell)
                tradeType = KuCoinApi.NetCore.Entities.TradeType.SELL;

            return tradeType;
        }

        public KuCoinApi.Net.Entities.Side KuCoinSideConverter(Contracts.Side side)
        {
            KuCoinApi.Net.Entities.Side kuCoinSide = side == Contracts.Side.Buy 
                ? KuCoinApi.Net.Entities.Side.BUY
                : KuCoinApi.Net.Entities.Side.SELL;

            return kuCoinSide;
        }

        public KuCoinApi.Net.Entities.Interval KuCoinIntervalConverter(Contracts.TimeInterval timeInterval)
        {
            switch(timeInterval)
            {
                case Contracts.TimeInterval.EightH:
                    return KuCoinApi.Net.Entities.Interval.EightH;
                case Contracts.TimeInterval.FifteenM:
                    return KuCoinApi.Net.Entities.Interval.FifteenM;
                case Contracts.TimeInterval.FiveM:
                    return KuCoinApi.Net.Entities.Interval.FiveM;
                case Contracts.TimeInterval.FourH:
                    return KuCoinApi.Net.Entities.Interval.FourH;
                case Contracts.TimeInterval.None:
                    return KuCoinApi.Net.Entities.Interval.None;
                case Contracts.TimeInterval.OneD:
                    return KuCoinApi.Net.Entities.Interval.OneD;
                case Contracts.TimeInterval.OneH:
                    return KuCoinApi.Net.Entities.Interval.OneH;
                case Contracts.TimeInterval.OneM:
                    return KuCoinApi.Net.Entities.Interval.OneM;
                case Contracts.TimeInterval.OneMo:
                    return KuCoinApi.Net.Entities.Interval.OneW;
                case Contracts.TimeInterval.OneW:
                    return KuCoinApi.Net.Entities.Interval.OneW;
                case Contracts.TimeInterval.SixH:
                    return KuCoinApi.Net.Entities.Interval.SixH;
                case Contracts.TimeInterval.ThirtyM:
                    return KuCoinApi.Net.Entities.Interval.ThirtyM;
                case Contracts.TimeInterval.ThreeD:
                    return KuCoinApi.Net.Entities.Interval.OneW;
                case Contracts.TimeInterval.ThreeM:
                    return KuCoinApi.Net.Entities.Interval.ThreeM;
                case Contracts.TimeInterval.TwelveH:
                    return KuCoinApi.Net.Entities.Interval.TwelveH;
                case Contracts.TimeInterval.TwoH:
                    return KuCoinApi.Net.Entities.Interval.TwoH;
                default:
                    return KuCoinApi.Net.Entities.Interval.None;
            }
        }

        #endregion KuCoin
    }
}
