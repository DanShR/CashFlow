
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace CashFlow.Common.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "admin@admin.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                var  s = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync(Environment.GetEnvironmentVariable("EMAIL"), Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

        }
    }
}