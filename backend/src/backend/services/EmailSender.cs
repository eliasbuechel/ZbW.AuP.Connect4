using backend.infrastructure;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace backend.services
{
    internal class EmailSender(EmailSettings emailSettings) : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(_emailSettings.FromAddress),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(new MailAddress(email));

            using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, LogContext.EMAIL_SENDER, $"Not able to send email to '{email}'", e);
            }
        }

        private readonly EmailSettings _emailSettings = emailSettings;
    }
}