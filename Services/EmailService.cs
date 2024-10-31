using Api.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Api.Services;

public class EmailService(ISendGridClient sendGridClient) : IEmailService
{
    private readonly ISendGridClient _sendGridClient = sendGridClient;

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
}
