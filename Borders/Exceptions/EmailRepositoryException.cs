using System;

namespace Borders.Exceptions
{
    [Serializable]
    public class StockQuotationException : Exception
    {
        public StockQuotationException(string errorMessage) : base(errorMessage) { }
    }
}
