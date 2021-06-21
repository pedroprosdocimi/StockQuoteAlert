using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace StockQuoteAlert.Config
{
    public class AppConfig
    {
        public IEnumerable<string> Emails { get; set; }
        public object SMTPConfiguration { get; init; }

        private void ReadAllSetting()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                Emails = appSettings["Emails"].Split(",").ToList();
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings.");
            }
        }
    }
}
