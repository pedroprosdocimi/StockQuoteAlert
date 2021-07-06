using Borders.Configs;
using Borders.Exceptions;
using Borders.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;

namespace Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly ApplicationConfig applicationConfig;
        private readonly ILogger logger;

        public EmailRepository(ApplicationConfig applicationConfig, ILogger<EmailRepository> logger)
        {
            this.applicationConfig = applicationConfig;
            this.logger = logger;
        }

        public void Post(string subject, string emailBody)
        {
            var receiverEmail = applicationConfig.SMTPConfiguration.EmailTo;
            try
            {
                MimeMessage email = CreateEmail(receiverEmail, subject, emailBody);
                SendEmail(email);             

                logger.LogInformation($"Email successfully sent to {receiverEmail}.");
            }
            catch (Exception e)
            {
                logger.LogInformation($"An error occurred when trying to send email to {receiverEmail}. Error: {e.Message}");
                throw new EmailRepositoryException($"Error when trying to send email to {receiverEmail}.");
            }
        }

        private void SendEmail(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            smtp.Connect(applicationConfig.SMTPConfiguration.SmtpHost, applicationConfig.SMTPConfiguration.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(applicationConfig.SMTPConfiguration.SmtpUser, applicationConfig.SMTPConfiguration.SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        private MimeMessage CreateEmail(string receiverEmail, string subject, string emailBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(applicationConfig.SMTPConfiguration.EmailFrom));
            email.To.Add(MailboxAddress.Parse(receiverEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            return email;
        }
    }
}
