using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BoxHub.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _settings;

        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
            if (string.IsNullOrWhiteSpace(_settings.Server) ||
                _settings.Port <= 0 ||
                string.IsNullOrWhiteSpace(_settings.UserName) ||
                string.IsNullOrWhiteSpace(_settings.Password) ||
                string.IsNullOrWhiteSpace(_settings.SenderEmail))
            {
                throw new InvalidOperationException("MailSettings is not configured.");
            }
        }

        public async Task<bool> SendEmailAsync(MailData mailData)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = mailData.EmailSubject,
                    Body = mailData.EmailBody,
                    IsBodyHtml = true
                };
                message.To.Add(mailData.EmailToId);

                using var client = new SmtpClient(_settings.Server, _settings.Port)
                {
                    EnableSsl = _settings.UseSsl,
                    Credentials = new NetworkCredential(_settings.UserName, _settings.Password)
                };

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService:SMTP] Error: {ex.Message}");
                return false;
            }
        }
    }
}
