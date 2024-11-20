using Api.Interfaces;
using Api.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Api.Services;


public class EmailService(IOptions<ApplicationSettings> appSettings, ISendGridClient sendGridClient, ILogger<EmailService> logger) : IEmailService
{
    private readonly ISendGridClient _sendGridClient = sendGridClient;
    private readonly ApplicationSettings _appSettings = appSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;
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
        _logger.LogInformation("Enviando correo electrónico con Gmail");

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("AESAN", _appSettings.Gmail.EmailFrom));
        emailMessage.To.Add(new MailboxAddress("", _appSettings.Gmail.EmailToDev));
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

            _logger.LogInformation("Correo electrónico enviado con Gmail");

        }
        catch (Exception ex) //todo add another try to send email
        {
            _logger.LogError(ex, "Error al enviar correo electrónico");
            throw new Exception("Error al enviar correo electrónico", ex);
        }
    }

    /// <summary>
    /// Envía un correo de bienvenida al auspiciador de una agencia
    /// </summary>
    /// <param name="userRequest">Datos del usuario y la agencia</param>
    /// <param name="temporaryPassword">Contraseña temporal asignada</param>
    public async Task SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword)
    {
        _logger.LogInformation("Enviando correo de bienvenida a la agencia");

        var subject = "¡Gracias por tu interés en formar parte del programa de AESAN!";
        var fullName = $"{userRequest.User.FirstName} {userRequest.User.FatherLastName}";

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <p>Estimado/a {fullName},</p>
                
                <p>Nos complace enormemente saber que estás interesado/a en formar parte de nuestros programas en AESAN. 
                Tu apoyo y participación son fundamentales para continuar con nuestra misión de [breve descripción de la misión o visión del programa].</p>
                
                <p>Para culminar tu proceso de registro y asegurar tu participación en el programa, te pedimos que sigas los siguientes pasos:</p>
                
                <ol>
                    <li>Haz clic en el botón <strong>Culminar Registro</strong> en el enlace que te proporcionamos.</li>
                    <li>Acceder con la contraseña temporera <strong>{temporaryPassword}</strong></li>
                </ol>
                
                <p>Si tienes alguna pregunta o necesitas asistencia adicional, no dudes en contactarnos. 
                Estamos aquí para ayudarte en cada paso del camino.</p>
                
                <p>Una vez más, gracias por su interés y confianza en AESAN. Juntos podemos lograr grandes cosas.</p>
                
                <p>Saludos cordiales,<br>
                {fullName}<br>
                {userRequest.User.AdministrationTitle}<br>
                AESAN<br>
                {userRequest.User.Email}<br>
                {userRequest.User.PhoneNumber}</p>
            </div>";

        await SendEmailWithGmailAsync(userRequest.User.Email, subject, htmlBody);
    }

    /// <summary>
    /// Envía un correo de confirmación de aprobación de auspiciador
    /// </summary>
    /// <param name="userRequest">Datos del usuario y la agencia</param>
    /// <param name="temporaryPassword">Contraseña temporal asignada</param>
    public async Task SendApprovalSponsorEmail(User user, string temporaryPassword)
    {
        _logger.LogInformation("Enviando correo de confirmación de aprobación de auspiciador");

        var subject = "¡Gracias por tu interés en formar parte del programa de AESAN!";
        var fullName = $"{user.FirstName} {user.FatherLastName}";

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <p>Estimado/a {fullName},</p>
                
                <p>Nos complace enormemente saber que estás interesado/a en formar parte de nuestros programas en AESAN. 
                Tu apoyo y participación son fundamentales para continuar con nuestra misión de [breve descripción de la misión o visión del programa].</p>
                
                <p>Para culminar tu proceso de registro y asegurar tu participación en el programa, te pedimos que sigas los siguientes pasos:</p>
                
                <ol>
                    <li>Haz clic en el botón <strong>Culminar Registro</strong> en el enlace que te proporcionamos.</li>
                    <li>Acceder con la contraseña temporera <strong>{temporaryPassword}</strong></li>
                </ol>
                
                <p>Si tienes alguna pregunta o necesitas asistencia adicional, no dudes en contactarnos. 
                Estamos aquí para ayudarte en cada paso del camino.</p>
                
                <p>Una vez más, gracias por su interés y confianza en AESAN. Juntos podemos lograr grandes cosas.</p>
                
                <p>Saludos cordiales,<br>
                {fullName}<br>
                {user.AdministrationTitle}<br>
                AESAN<br>
                {user.Email}<br>
                {user.PhoneNumber}</p>
            </div>";

#if !DEBUG
        await SendEmailWithGmailAsync(user.Email, subject, htmlBody);
#endif

    }

    /// <summary>
    /// Envía un correo de confirmación de denegación de auspiciador
    /// </summary>
    /// <param name="userRequest">Datos del usuario y la agencia</param>
    public async Task SendDenialSponsorEmail(User user, string rejectionReason)
    {
        _logger.LogInformation("Enviando correo de confirmación de denegación de auspiciador");

        var subject = "Actualización sobre tu aplicación al programa de AESAN";
        var fullName = $"{user.FirstName} {user.FatherLastName}";

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <p>Estimado/a {fullName},</p>
                
                <p>Lamentamos informarte que tu aplicación para participar en el programa de AESAN ha sido rechazada por la siguiente razón: {rejectionReason}</p>
                
                <p>Si tienes alguna pregunta o necesitas aclaraciones adicionales, no dudes en contactarnos. 
                Estamos aquí para ayudarte y proporcionar más información si lo necesitas.</p>
                
                <p>Agradecemos sinceramente tu interés en AESAN y te deseamos éxito en tus futuros proyectos.</p>
                
                <p>Atentamente,<br>
                {fullName}<br>
                {user.AdministrationTitle}<br>
                Equipo de AESAN<br>
                {user.Email}<br>
                {user.PhoneNumber}</p>
            </div>";

#if !DEBUG
        await SendEmailWithGmailAsync(user.Email, subject, htmlBody);
#endif

    }

}
