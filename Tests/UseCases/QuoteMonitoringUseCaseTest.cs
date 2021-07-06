using Borders.Constants;
using Borders.Dtos;
using Borders.Exceptions;
using Borders.Repositories;
using Borders.Validators;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using UseCases;
using Xunit;

namespace Tests.UseCases
{
    public class QuoteMonitoringUseCaseTest
    {
        private class Setup
        {
            public Setup()
            {
                stockQuotationRepository = new Mock<IStockQuotationRepository>();
                emailRepository = new Mock<IEmailRepository>();
                logger = new Mock<ILogger<QuoteMonitoringUseCase>>();
                validator = new QuoteMonitoringRequestValidator();
                useCase = () => new QuoteMonitoringUseCase(
                    stockQuotationRepository.Object,
                    emailRepository.Object,
                    logger.Object,
                    validator);
            }

            public Mock<IStockQuotationRepository> stockQuotationRepository { get; }
            public Mock<IEmailRepository> emailRepository { get; }
            public Mock<ILogger<QuoteMonitoringUseCase>> logger { get; }
            public QuoteMonitoringRequestValidator validator { get; }
            public Func<QuoteMonitoringUseCase> useCase { get; }
        }

        [Theory]
        [InlineData(18.0f, "sell")]
        [InlineData(9.0f, "buy")]
        public async Task Execute_WhenActualStockQuotationIsNotInsideLimit_SendEmail(float actualStockQuotation, string action)
        {
            var setup = new Setup();

            var stockSymbol = "PETR4";

            var subject = EmailConstants.StockQuoteEmailSubject;
            var htmlBody = string.Format(EmailConstants.StockQuoteEmailBody, stockSymbol, actualStockQuotation, action);

            setup.stockQuotationRepository.Setup(s => s.Get(stockSymbol)).ReturnsAsync(new StockResult(actualStockQuotation));
            setup.emailRepository.Setup(s => s.Post(subject, htmlBody));

            var useCase = setup.useCase();

            var lowerLimit = 10.0f;
            var higherLimit = 15.0f;
            var request = new QuoteMonitoringRequest(stockSymbol, lowerLimit, higherLimit);
            await useCase.Execute(request);

            setup.stockQuotationRepository.Verify(it => it.Get(stockSymbol), Times.Once);
            setup.emailRepository.Verify(it => it.Post(subject, htmlBody), Times.Once);
        }

        [Theory]
        [InlineData("", 17.9f, 18.9f, "Invalid Input: StockSymbol is invalid")]
        [InlineData("PETR4", null, 18.9f, "Invalid Input: MinValue should be a decimal value.")]
        [InlineData("PETR4", 17.9f, null, "Invalid Input: MaxValue should be a decimal value.")]
        [InlineData("PETR4", 18.9f, 17.9f, "Invalid Input: MaxValue should be greater than MinValue.")]
        public async Task Execute_WhenRequestIsInvalid_ReturnException(string stockSymbol, float MinValue, float MaxValue, string expectedErrorMessage)
        {
            var setup = new Setup();
            var useCase = setup.useCase();
            var request = new QuoteMonitoringRequest(stockSymbol, MinValue, MaxValue);

            try
            {
                await useCase.Execute(request);
            }
            catch (Exception e)
            {
                e.Message.Should().Be(expectedErrorMessage);
            }
        }

        [Fact]
        public async Task Execute_WhenActualStockQuotationIsInsideLimit_VoidSucess()
        {
            var setup = new Setup();

            var stockSymbol = "PETR4";

            setup.stockQuotationRepository.Setup(s => s.Get(stockSymbol)).ReturnsAsync(new StockResult(13.0f));

            var useCase = setup.useCase();

            var lowerLimit = 10.0f;
            var higherLimit = 15.0f;
            var request = new QuoteMonitoringRequest(stockSymbol, lowerLimit, higherLimit);
            await useCase.Execute(request);

            setup.stockQuotationRepository.Verify(it => it.Get(stockSymbol), Times.Once);
            setup.emailRepository.Verify(it => it.Post(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenActualStockQuotationIsInsideLimit_StockQuotationRepositoryError()
        {
            var setup = new Setup();

            var stockSymbol = "PETR4";

            var errorMessage = "Unexpected error at StockQuotationRepository";
            setup.stockQuotationRepository.Setup(s => s.Get(stockSymbol)).ThrowsAsync(
                new StockQuotationException(errorMessage));

            var useCase = setup.useCase();

            var lowerLimit = 10.0f;
            var higherLimit = 15.0f;
            var request = new QuoteMonitoringRequest(stockSymbol, lowerLimit, higherLimit);

            try
            {
                await useCase.Execute(request);
            }
            catch (Exception e)
            {
                e.Message.Should().Be(errorMessage);
                setup.stockQuotationRepository.Verify(it => it.Get(stockSymbol), Times.Once);
                setup.emailRepository.Verify(it => it.Post(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            }
        }

        [Fact]
        public async Task Execute_WhenActualStockQuotationIsInsideLimit_EmailRepositoryError()
        {
            var setup = new Setup();

            var stockSymbol = "PETR4";
            var actualStockQuotation = 17.0f;

            var subject = EmailConstants.StockQuoteEmailSubject;
            var htmlBody = string.Format(EmailConstants.StockQuoteEmailBody, stockSymbol, actualStockQuotation, "sell");

            setup.stockQuotationRepository.Setup(s => s.Get(stockSymbol)).ReturnsAsync(new StockResult(actualStockQuotation));

            var errorMessage = $"Error when trying to send email to test@test.com.";
            setup.emailRepository.Setup(s => s.Post(subject, htmlBody)).Throws(new EmailRepositoryException(errorMessage));

            var useCase = setup.useCase();

            var lowerLimit = 10.0f;
            var higherLimit = 15.0f;
            var request = new QuoteMonitoringRequest(stockSymbol, lowerLimit, higherLimit);

            try
            {
                await useCase.Execute(request);
            }
            catch (Exception e)
            {
                e.Message.Should().Be(errorMessage);
                setup.stockQuotationRepository.Verify(it => it.Get(stockSymbol), Times.Once);
                setup.emailRepository.Verify(it => it.Post(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            }
        }
    }
}
