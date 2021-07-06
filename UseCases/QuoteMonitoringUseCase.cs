using Borders.Constants;
using Borders.Dtos;
using Borders.Exceptions;
using Borders.Repositories;
using Borders.UseCases;
using Borders.Validators;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UseCases
{
    public class QuoteMonitoringUseCase : IQuoteMonitoringUseCase
    {
        private readonly IStockQuotationRepository stockQuotationRepository;
        private readonly IEmailRepository emailRepository;
        private readonly ILogger<QuoteMonitoringUseCase> logger;
        private readonly IValidator<QuoteMonitoringRequest> validator;

        public QuoteMonitoringUseCase(IStockQuotationRepository stockQuotationRepository, IEmailRepository emailRepository, 
            ILogger<QuoteMonitoringUseCase> logger, IValidator<QuoteMonitoringRequest> validator)
        {
            this.stockQuotationRepository = stockQuotationRepository;
            this.emailRepository = emailRepository;
            this.logger = logger;
            this.validator = validator;
        }

        public async Task Execute(QuoteMonitoringRequest request)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
                throw new Exception($"Invalid Input: {validationResult.Errors.First().ErrorMessage}");

            try
            {
                var stockSymbol = request.StockSymbol;
                var stockQuotation = await stockQuotationRepository.Get(stockSymbol);
                var actualStockQuotation = stockQuotation.Price;

                if (actualStockQuotation > request.MaxValue)
                {
                    emailRepository.Post(
                        subject: EmailConstants.StockQuoteEmailSubject,
                        html: string.Format(EmailConstants.StockQuoteEmailBody, stockSymbol, actualStockQuotation, "sell"));
                }
                else if (actualStockQuotation < request.MinValue)
                {
                    emailRepository.Post(
                        subject: EmailConstants.StockQuoteEmailSubject,
                        html: string.Format(EmailConstants.StockQuoteEmailBody, stockSymbol, actualStockQuotation, "buy"));
                }

                logger.LogInformation("The Quote Monitoring Use Case finished successfully.");
            }
            catch(StockQuotationException e)
            {
                logger.LogInformation(e.Message);
            }
        }
    }
}