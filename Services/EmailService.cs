using SendGrid;
using SendGrid.Helpers.Mail;

namespace Api.Services
{
    public class EmailService(IConfiguration configuration)
    {
        private readonly string _apiKey = configuration["SendGrid:ApiKey"];

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("your_email@example.com", "Your Name");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
