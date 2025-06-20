using GraduateProject_Core.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SendGridEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(_config["SendGrid:SenderEmail"], _config["SendGrid:SenderName"]);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            var response = await client.SendEmailAsync(msg);

            if ((int)response.StatusCode >= 400)
            {
                var error = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"SendGrid Error: {error}");
            }
        }
    }
}
