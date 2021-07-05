using Borders.Configs;
using Borders.Dtos;
using Borders.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Repositories
{
    public class StockQuotationRepository : IStockQuotationRepository
    {
        private readonly HttpClient httpClient;
        private readonly ApplicationConfig applicationConfig;
        private readonly ILogger logger;

        public StockQuotationRepository(HttpClient httpClient, ApplicationConfig applicationConfig, 
            ILogger<StockQuotationRepository> logger)
        {
            this.httpClient = httpClient;
            this.applicationConfig = applicationConfig;
            this.logger = logger;
        }

        public async Task<StockResult> Get(string request)
        {
            using var httpResponse = await httpClient.GetAsync($"/finance/stock_price?key={applicationConfig.ApiKey}&symbol={request}");

            if (!httpResponse.IsSuccessStatusCode)
                throw new Exception($"Error when trying to get {request} price information. " +
                    $"ErrorCode: {httpResponse.StatusCode}.");
               
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            return GetStockQuoteInfo(jsonResponse, request);
            
                                  
        }

        private StockResult GetStockQuoteInfo(string jsonResponse, string request)
        {
            object resultObject;
            object stocksInfoObject;

            var resultDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);            
            resultDictionary.TryGetValue("results", out resultObject);

            var stocksInfoPayload = JsonConvert.SerializeObject(resultObject);
            var stocksInfoDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(stocksInfoPayload);            
            stocksInfoDictionary.TryGetValue(request, out stocksInfoObject);

            var requestedStockInfo = JsonConvert.SerializeObject(stocksInfoObject);
            
            return JsonConvert.DeserializeObject<StockResult>(requestedStockInfo);
        }
    }
}
