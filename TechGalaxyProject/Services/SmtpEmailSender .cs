using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TechGalaxyProject.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var apiKey = _config["Brevo:ApiKey"];
            var senderEmail = _config["Brevo:SenderEmail"];
            var senderName = _config["Brevo:SenderName"];

            var plainText = "Please check your email in HTML format to reset your password.";

            var requestBody = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { email = toEmail } },
                subject = subject,
                htmlContent = htmlMessage,
                textContent = plainText,
                headers = new Dictionary<string, string>
                {
                    { "X-Mailin-custom", "TechGalaxyMail" }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

            var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(" Failed to send via Brevo API:");
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Response: {responseBody}");
                throw new Exception($"Failed to send email via Brevo: {response.StatusCode}");
            }

            Console.WriteLine(" Email sent successfully via Brevo API");
        }

        public string GenerateResetPasswordEmailBody(string userName, string resetUrl)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2>Reset Your Password</h2>
                    <p>Hello <strong>{userName}</strong>,</p>
                    <p>You requested a password reset. Please click the button below:</p>
                    <p>
                        <a href='{resetUrl}' style='
                            background-color: #4CAF50;
                            color: white;
                            padding: 10px 20px;
                            text-decoration: none;
                            border-radius: 5px;
                            display: inline-block;'>Reset Password</a>
                    </p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p style='font-size: 12px; color: gray;'>– Tech Galaxy Team</p>
                </div>";
        }
    }
}
