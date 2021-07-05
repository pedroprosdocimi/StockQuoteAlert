namespace Borders.Configs
{
    public class ApplicationConfig
    {
        public string ApiKey { get; init; }
        public SmtpConfiguration SMTPConfiguration { get; init; }
        public ApiConfiguration HGConsole { get; init; }

        public class ApiConfiguration
        {
            public string BaseUrl { get; init; }
        }
    }
}
