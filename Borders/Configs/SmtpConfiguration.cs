namespace Borders.Configs
{
    public record SmtpConfiguration
    {
        public string EmailFrom { get; init; }
        public string EmailTo { get; init; }
        public string SmtpHost { get; init; }
        public int SmtpPort { get; init; }
        public string SmtpUser { get; init; }
        public string SmtpPass { get; init; }
    }
}
