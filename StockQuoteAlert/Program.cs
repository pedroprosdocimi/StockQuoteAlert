using Batch.Controllers;
using Borders.Configs;
using Borders.Dtos;
using Borders.Repositories;
using Borders.UseCases;
using Borders.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using System;
using UseCases;

namespace StockQuoteAlert
{
    class Program
    {
        private readonly QuoteAlertController quoteAlertController;

        public Program(QuoteAlertController quoteAlertController)
        {
            this.quoteAlertController = quoteAlertController;
        }

        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetRequiredService<Program>().Run();
        }

        public void Run()
        {
            try
            {
                var input = Console.ReadLine();
                var inputList = input.Split(" ");
                var request = new QuoteMonitoringRequest(inputList[0], float.Parse(inputList[1]), float.Parse(inputList[2]));

                while (true)
                {
                    quoteAlertController.Monitor(request).Wait();
                    System.Threading.Thread.Sleep(300000); //waiting 5 minutes to get the value.
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var applicationConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build().Get<ApplicationConfig>();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<Program>();
                    services.AddSingleton<IValidator<QuoteMonitoringRequest>, QuoteMonitoringRequestValidator>();
                    services.AddSingleton<ApplicationConfig>(applicationConfig);
                    services.AddSingleton<QuoteAlertController>();
                    services.AddSingleton<IQuoteMonitoringUseCase, QuoteMonitoringUseCase>();
                    services.AddHttpClient<IStockQuotationRepository, StockQuotationRepository>(client =>
                    {
                        client.BaseAddress = new Uri(applicationConfig.HGConsole.BaseUrl);
                    });
                    services.AddSingleton<IEmailRepository, EmailRepository>();
                    services.AddLogging();
                });
        }
    }
}
