using ExchangeHub.Contracts;
using System;
using Xunit;

namespace ExchangeHub.Tests
{
    public class ExchangeHubTests : IDisposable
    {
        private ExchangeHub hub;

        public ExchangeHubTests()
        {
            var fileRepo = new FileRepository.FileRepository();
            var apiInfo = fileRepo.GetDataFromFile<ApiInformation>("config.json");
            hub = new ExchangeHub(Exchange.Binance, apiInfo.ApiKey, apiInfo.ApiSecret);
        }

        public void Dispose()
        {
        }

        [Fact]
        public void GetBalances()
        {
            var balances = hub.GetBalance();

            Assert.NotEmpty(balances);
        }

        [Fact]
        public void GetOrders()
        {
            var pair = "XLMBTC";
            var orders = hub.GetOrders(pair);

            Assert.NotEmpty(orders);
        }
    }
}
