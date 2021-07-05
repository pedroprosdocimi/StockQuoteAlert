using Borders.Dtos;
using System.Threading.Tasks;

namespace Borders.Repositories
{
    public interface IStockQuotationRepository
    {
        public Task<StockResult> Get(string request);
    }
}
