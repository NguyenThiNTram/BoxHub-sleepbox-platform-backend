using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace BoxHub.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public EmailService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _apiKey = config["Resend:ApiKey"];
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("Resend:ApiKey không được rỗng.");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<bool> SendEmailAsync(MailData mailData)
        {
            try
            {
                var payload = new
                {
                    from = "BoxHub <noreply@notify.boxhub-sleepbox.com>", // test ban đầu
                    to = new[] { mailData.EmailToId },
                    subject = mailData.EmailSubject,
                    html = mailData.EmailBody
                };

                var response = await _httpClient.PostAsJsonAsync("https://api.resend.com/emails", payload);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[Resend] Status: {response.StatusCode}, Body: {body}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error: {ex.Message}");
                return false;
            }
        }
    }
}