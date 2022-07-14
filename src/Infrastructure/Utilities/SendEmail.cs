using Core.Common.Interfaces;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Serilog;

namespace Infrastructure.Utilities
{
    public interface ISendEmail : IScopedService
    {
        Task<bool> SendEmailAsync(string? toEmail, string? subject, string body);
    }

    public class SendEmail : ISendEmail
    {
        private readonly IConfiguration Configuration;

        public SendEmail(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string? toEmail, string? subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Configuration["MailSettings:Mail"]));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            BodyBuilder builder = new ();
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(Configuration["MailSettings:Host"], int.Parse(Configuration["MailSettings:Port"]), SecureSocketOptions.Auto);
                smtp.Authenticate(Configuration["MailSettings:Mail"], Configuration["MailSettings:Password"]);
                await smtp.SendAsync(email);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex,ex.GetBaseException().ToString());
                return false;
            }
            finally
            {
                smtp.Disconnect(true);
            }
            return true;
        }
    }
}