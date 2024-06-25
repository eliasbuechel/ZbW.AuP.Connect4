using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace backend.services
{
    internal class EmailSender : IEmailSender
    {
        public EmailSender(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

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

            using (var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception) { }
            }
        }

        private readonly EmailSettings _emailSettings;
    }
}