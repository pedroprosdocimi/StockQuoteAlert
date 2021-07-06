using Borders.Dtos;
using Borders.Validators;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Tests.BorderTests
{
    public class QuoteMonitoringRequestValidatorTest
    {
        private readonly QuoteMonitoringRequestValidator validator = new QuoteMonitoringRequestValidator();
        private readonly QuoteMonitoringRequest request = new QuoteMonitoringRequest("PETR4", 10.0f, 20.0f);

        [Fact]
        public void Validate_WhenStockSymbolIsEmpty_IsInvalid()
        {
            var result = validator.Validate(request with { StockSymbol = string.Empty });

            result.Errors.First().ErrorMessage.Should().Be("StockSymbol is invalid");
        }

        [Fact]
        public void Validate_WhenMinValueIsEmpty_IsInvalid()
        {
            var result = validator.Validate(request with { MinValue = 0.0f });

            result.Errors.First().ErrorMessage.Should().Be("MinValue should be a decimal value.");
        }

        [Fact]
        public void Validate_WhenMaxValueIsEmpty_IsInvalid()
        {
            var result = validator.Validate(request with { MaxValue = 0.0f });

            result.Errors.First().ErrorMessage.Should().Be("MaxValue should be a decimal value.");
        }

        [Fact]
        public void Validate_WhenMaxValueIsSmallerThanMinValue_IsInvalid()
        {
            var result = validator.Validate(request with { MinValue = 23.0f });

            result.Errors.First().ErrorMessage.Should().Be("MaxValue should be greater than MinValue.");
        }
    }
}
