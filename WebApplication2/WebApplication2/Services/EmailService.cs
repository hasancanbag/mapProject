using System.Net.Mail;
using System.Net;
using WebApplication2.Interfaces;

namespace WebApplication2.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"])))
            {
                client.Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["Email:From"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);
                await client.SendMailAsync(mailMessage);
            }
        }
    }

}
