using Borders.Configs;
using Borders.Constants;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Repositories;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Repositories
{
    public class EmailRepositoryTest
    {
        [Fact]
        public async Task GetStockPrice_ReturnSuccess_WhenStockExists()
        {
            var subjectRequest = EmailConstants.StockQuoteEmailSubject;
            var htmlBodyRequest = string.Format(EmailConstants.StockQuoteEmailBody, "PETR4", 15.0f, "sell");

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

            var appConfig = new ApplicationConfig()
            {
                SMTPConfiguration = new SmtpConfiguration()
                {
                    EmailFrom = "from@test.com",
                    EmailTo = "to@test.com",
                    SmtpHost = "host.com",
                    SmtpPass = "p@ssw0rd",
                    SmtpUser = "user",
                    SmtpPort = 587
                }
            };

            var logger = new Mock<ILogger<EmailRepository>>();

            var emailRepository = new EmailRepository(appConfig, logger.Object);
        }
    }
}
