using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace backend.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IConfiguration configuration)
        {
            _smtpClient = new SmtpClient(configuration["Smtp:Host"])
            {
                Port = int.Parse(configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]),
                EnableSsl = true,
            };
            _fromAddress = configuration["Smtp:FromAddress"];
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage(_fromAddress, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }

        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;
    }
}

