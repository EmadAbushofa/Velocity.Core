using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Velocity.Core.AspNetCore.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(
            string host,
            int port,
            string password,
            string sentFrom
        )
        {
            Host = host;
            Port = port;
            Password = password;
            SentFrom = sentFrom;
        }

        public string Host { get; protected set; }
        public int Port { get; protected set; }
        public string Password { get; protected set; }
        public string SentFrom { get; protected set; }

        public virtual async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(SentFrom, Password)
            };
            await client.SendMailAsync(new MailMessage()
            {
                Body = htmlMessage,
                IsBodyHtml = true,
                From = new MailAddress(SentFrom),
                Subject = subject,
                To =
                {
                    email
                }
            });
        }
    }
}
