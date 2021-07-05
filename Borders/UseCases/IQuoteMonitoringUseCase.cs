using Borders.Dtos;
using System.Threading.Tasks;

namespace Borders.UseCases
{
    public interface IQuoteMonitoringUseCase
    {
        public Task Execute(QuoteMonitoringRequest request);
    }
}
