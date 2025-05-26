using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using atgv.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace atgv.Infrastructure
{
    public class FakeEmailService(ILogger<FakeEmailService> _logger, IOptions<SmtpSettings> _smtpSettings)
        : IEmailService
    {
        public Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var smtpSettings = _smtpSettings.Value;

            _logger.LogInformation("Fake email to {to} from {from} with subject {subject} and body {body}", to, smtpSettings.SenderEmail, subject, body);
            return Task.FromResult(true);
        }

    }
    public class EmailService(ILogger<EmailService> _logger, IOptions<SmtpSettings> _smtpSettings)
        : IEmailService
    {

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpSettings = _smtpSettings.Value;

                using (var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings.SenderEmail, smtpSettings.SenderName),
                        Subject = subject,
                        Body = body
                    };
                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);

                    _logger.LogInformation($"Email sent successfully to {to} for subject '{subject}'.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred when sending email to {to}: {ex.Message}. Trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}