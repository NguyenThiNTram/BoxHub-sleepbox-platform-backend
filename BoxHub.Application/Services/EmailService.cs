using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;

        public EmailService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            var apiKey = config["Resend:ApiKey"]; // Đọc từ appsettings

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("RESEND_API_KEY not null.");

            _httpClient.BaseAddress = new Uri("https://api.resend.com/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<bool> SendEmailAsync(MailData mailData)
        {
            try
            {
                var payload = new
                {
                    from = "onboarding@resend.dev", // Nhớ đổi thành domain thật của bạn sau khi verify trên Resend
                    to = new[] { mailData.EmailToId },
                    subject = mailData.EmailSubject,
                    html = mailData.EmailBody
                };

                var response = await _httpClient.PostAsJsonAsync("emails", payload);
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
