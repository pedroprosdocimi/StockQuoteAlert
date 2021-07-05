using Borders.Dtos;
using FluentValidation;

namespace Borders.Validators
{
    public class QuoteMonitoringRequestValidator : AbstractValidator<QuoteMonitoringRequest>
    {
        public QuoteMonitoringRequestValidator()
        {
            RuleFor(x => x.StockSymbol).NotEmpty().WithMessage("StockSymbol is invalid");
            RuleFor(x => x.MaxValue).NotEmpty().WithMessage("MaxValue should be a decimal value.");
            RuleFor(x => x.MinValue).NotEmpty().WithMessage("MinValue should be a decimal value.");
            RuleFor(x => x.MaxValue).GreaterThan(x => x.MinValue).WithMessage("MaxValue should be greater than MinValue.");
        }
    }
}
