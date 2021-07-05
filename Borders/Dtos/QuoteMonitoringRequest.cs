namespace Borders.Dtos
{
    public record QuoteMonitoringRequest(
        string StockSymbol,
        float MinValue,
        float MaxValue
    );
}
