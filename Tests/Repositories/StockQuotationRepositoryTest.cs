using Borders.Configs;
using Borders.Dtos;
using Borders.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Repositories;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UseCases;
using Xunit;

namespace Tests.Repositories
{
    public class StockQuotationRepositoryTest
    {
        [Fact]
        public async Task GetStockPrice_ReturnSuccess_WhenStockExists()
        {
            var request = "PETR4";
            var apiResponse = new
            {
                results = new
                {
                    PETR4 = new
                    {
                        price = 15.0f
                    }
                }
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(apiResponse))
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(response);

            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://test.com") };
            var appConfig = new ApplicationConfig();
            var logger = new Mock<ILogger<StockQuotationRepository>>();

            var stockQuotationRepository = new StockQuotationRepository(httpClient, appConfig, logger.Object);
            var repositoryResponse = await stockQuotationRepository.Get(request);
            var expectedResponse = new StockResult(15.0f);

            repositoryResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetStockPrice_ReturnStockQuotationException_WhenStockDoesNotExists()
        {
            var request = "PETR4";

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(JsonConvert.SerializeObject(new { message = "Not Found" }))
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(response);

            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://test.com") };
            var appConfig = new ApplicationConfig();
            var logger = new Mock<ILogger<StockQuotationRepository>>();

            var stockQuotationRepository = new StockQuotationRepository(httpClient, appConfig, logger.Object);
            try
            {
                await stockQuotationRepository.Get(request);
            }
            catch (StockQuotationException e)
            {
                e.Message.Should().BeEquivalentTo($"Error when trying to get {request} price information. " +
                    $"ErrorCode: {HttpStatusCode.NotFound}.");
            }
        }

        [Fact]
        public async Task GetStockPrice_ReturnStockQuotationException_WhenContractIsBroken()
        {
            var request = "PETR4";
            var apiResponse = new
            {
                PETR4 = new
                {
                    price = 15.0f
                }
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(apiResponse))
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(response);

            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://test.com") };
            var appConfig = new ApplicationConfig();
            var logger = new Mock<ILogger<StockQuotationRepository>>();

            var stockQuotationRepository = new StockQuotationRepository(httpClient, appConfig, logger.Object);
            try
            {
                await stockQuotationRepository.Get(request);
            }
            catch (StockQuotationException e)
            {
                e.Message.Should().BeEquivalentTo("Unexpected error at StockQuotationRepository");
            }
        }
    }
}
