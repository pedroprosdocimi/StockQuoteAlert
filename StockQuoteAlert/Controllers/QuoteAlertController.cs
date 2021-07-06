using Borders.Dtos;
using Borders.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Batch.Controllers
{
    public class QuoteAlertController : Controller
    {
        private readonly IQuoteMonitoringUseCase quoteMonitoringUseCase;

        public QuoteAlertController(IQuoteMonitoringUseCase quoteMonitoringUseCase)
        {
            this.quoteMonitoringUseCase = quoteMonitoringUseCase;
        }

        public async Task Monitor(QuoteMonitoringRequest request) =>
            await quoteMonitoringUseCase.Execute(request);
    }
}
