using Borders.Dtos;
using Borders.UseCases;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Batch.Controllers
{
    public class QuoteAlertController : Controller
    {
        private readonly IQuoteMonitoringUseCase quoteMonitoringUseCase;
        private readonly IValidator<QuoteMonitoringRequest> validator;

        public QuoteAlertController(IQuoteMonitoringUseCase quoteMonitoringUseCase, IValidator<QuoteMonitoringRequest> validator)
        {
            this.quoteMonitoringUseCase = quoteMonitoringUseCase;
            this.validator = validator;
        }

        public async Task Monitor()
        {
            var input = Console.ReadLine();
            var inputList = input.Split(" ");
            var request = new QuoteMonitoringRequest(inputList[0], float.Parse(inputList[1]), float.Parse(inputList[2]));

            var result = validator.Validate(request);

            if (!result.IsValid)
                throw new Exception($"Invalid Input: {result.Errors.First().ErrorMessage}");

            while (true)
            {
                await quoteMonitoringUseCase.Execute(request);
                System.Threading.Thread.Sleep(300000); //waiting 5 minutes to search get the value.
            }
        }
    }
}
