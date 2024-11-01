using Api.Interfaces;
using Api.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Api.Services;


public class EmailService(IOptions<ApplicationSettings> appSettings, ISendGridClient sendGridClient) : IEmailService
{
    private readonly ISendGridClient _sendGridClient = sendGridClient;
    private readonly ApplicationSettings _appSettings = appSettings.Value;

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("noreply@yourdomain.com", "Your App Name"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };

        msg.AddTo(new EmailAddress(email));

        await _sendGridClient.SendEmailAsync(msg);
    }

    public async Task SendTemporaryPasswordEmail(string email, string temporaryPassword)
    {
        // Implementar lógica para enviar correo electrónico
        // Puedes usar el servicio de correo electrónico que prefieras, como SendGrid
        // Ejemplo usando SendGrid:
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("noreply@yourdomain.com", "Your App Name"),
            Subject = "Tu contraseña temporal",
            PlainTextContent = $"Tu contraseña temporal es: {temporaryPassword}",
            HtmlContent = $"<strong>Tu contraseña temporal es: {temporaryPassword}</strong>"
        };

        msg.AddTo(new EmailAddress(email));

        var response = await _sendGridClient.SendEmailAsync(msg);
        // Manejar la respuesta si es necesario
    }

    /// <summary>
    /// Envia un correo electrónico usando Gmail
    /// </summary>
    /// <param name="email">El correo electrónico del destinatario</param>
    /// <param name="subject">El asunto del correo electrónico</param>
    /// <param name="message">El mensaje del correo electrónico</param>
    public async Task SendEmailWithGmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Your App Name", "noreply@yourdomain.com"));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = message,
            HtmlBody = $"<strong>{message}</strong>"
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();

        try
        {
            var client = new MailKit.Net.Smtp.SmtpClient();

            client.AuthenticationMechanisms.Remove("XOAUTH2");

            await client.ConnectAsync(_appSettings.Gmail.SmtpServer, _appSettings.Gmail.SmtpServerPort, MailKit.Security.SecureSocketOptions.Auto);
            await client.AuthenticateAsync(_appSettings.Gmail.EmailFrom, _appSettings.Gmail.SmtpPass);

            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

        }
        catch (Exception ex) //todo add another try to send email
        {
            var e = ex;
            throw;
        }
    }
}
