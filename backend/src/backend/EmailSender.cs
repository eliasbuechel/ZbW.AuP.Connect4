using backend.database;
using Microsoft.AspNetCore.Identity;


namespace backend
{
    public class EmailSender : IEmailSender<PlayerIdentity>
    {

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
        Task IEmailSender<PlayerIdentity>.SendConfirmationLinkAsync(PlayerIdentity user, string email, string confirmationLink)
        {
            return Task.CompletedTask;
        }
        Task IEmailSender<PlayerIdentity>.SendPasswordResetCodeAsync(PlayerIdentity user, string email, string resetCode)
        {
            return Task.CompletedTask;
        }
        Task IEmailSender<PlayerIdentity>.SendPasswordResetLinkAsync(PlayerIdentity user, string email, string resetLink)
        {
            return Task.CompletedTask;
        }
    }
}