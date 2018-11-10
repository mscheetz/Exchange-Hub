using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeHub.Contracts
{
    public enum Exchange
    {
        Binance,
        Bittrex,
        CoinbasePro,
        CoinEx,
        KuCoin,
        Switcheo
    }

    public enum Side
    {
        Buy,
        Sell
    }

    public enum OrderStatus
    {
        Open,
        Filled,
        PartialFill,
        Canceled
    }

    public enum TimeInterval
    {
        None = 0,
        OneM = 1,
        ThreeM = 2,
        FiveM = 3,
        FifteenM = 4,
        ThirtyM = 5,
        OneH = 6,
        TwoH = 7,
        FourH = 8,
        SixH = 9,
        EightH = 10,
        TwelveH = 11,
        OneD = 12,
        ThreeD = 13,
        OneW = 14,
        OneMo = 15
    }
}
