using DateTimeHelpers;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeHub.Proxies
{
    public class ProxyBase
    {
        private DateTimeHelper _dtHelper;

        public ProxyBase()
        {
            this._dtHelper = new DateTimeHelper();

            #region Binance

            TinyMapper.Bind<Binance.NetCore.Entities.Balance, ExchangeHub.Contracts.Balance>(config =>
            {
                config.Bind(source => source.asset, target => target.Symbol);
                config.Bind(source => source.free, target => target.Available);
                config.Bind(source => source.locked, target => target.Frozen);
            });

            TinyMapper.Bind<Binance.NetCore.Entities.TradeResponse, ExchangeHub.Contracts.OrderResponse>(config =>
            {
                config.Bind(source => source.executedQty, target => target.FilledQuantity);
                config.Bind(source => source.orderId, target => target.OrderId);
                config.Bind(source => source.origQty, target => target.OrderQuantity);
                config.Bind(source => source.price, target => target.Price);
                config.Bind(source => source.side, target => target.Side);
                config.Bind(source => source.executedQty, target => target.FilledQuantity);
                config.Bind(source => source.symbol, target => target.Symbol);
            });

            #endregion Binance
        }

        #region Binance
        public Contracts.OrderResponse BinanceOrderResponseToOrderResponse(Binance.NetCore.Entities.OrderResponse binanceResponse)
        {
            var orderResponse = new Contracts.OrderResponse
            {
                FilledQuantity = binanceResponse.executedQty,
                OrderId = binanceResponse.orderId.ToString(),
                OrderQuantity = binanceResponse.origQty,
                OrderStatus = BinanceOrderStatusConverter(binanceResponse.status),
                Price = binanceResponse.price,
                Side = BinanceTradeTypeConverter(binanceResponse.side),
                StopPrice = binanceResponse.stopPrice,
                Symbol = binanceResponse.symbol,
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
                Side = BinanceTradeTypeConverter(tradeResponse.side),
                Symbol = tradeResponse.symbol,
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
                PriceChangePercent = double.Parse(tick.priceChangePercent),
                Symbol = tick.symbol,
                Volume = decimal.Parse(tick.volume),
                WeightedAvgPrice = decimal.Parse(tick.weightedAvgPrice),
            };

            return ticker;
        }

        public Contracts.OrderStatus BinanceOrderStatusConverter(Binance.NetCore.Entities.OrderStatus binanceStatus)
        {
            Contracts.OrderStatus orderStatus;

            switch(binanceStatus)
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
    }
}
