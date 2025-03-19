using Api.Interfaces;
using Api.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
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

#if DEBUG || LOCAL
        emailMessage.To.Add(new MailboxAddress("", _appSettings.Gmail.EmailToDev));
#else
        emailMessage.To.Add(new MailboxAddress("", email));
#endif

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

    /// <summary>
    /// Envía un correo electrónico notificando la asignación de una agencia a un usuario
    /// </summary>
    /// <param name="user">Usuario al que se le asignó la agencia</param>
    /// <param name="agency">Agencia asignada</param>
    public async Task SendAgencyAssignmentEmail(DTOUser user, DTOAgency agency)
    {
        try
        {
            _logger.LogInformation($"Preparando correo de asignación de agencia para {user.Email}");

            var subject = "Asignación de Agencia en NUTRE";
            var body = $@"
                <h2>Asignación de Agencia</h2>
                <p>Estimado/a {user.FirstName} {user.FatherLastName},</p>
                <p>Le informamos que se le ha asignado la siguiente agencia en el sistema NUTRE:</p>
                <ul>
                    <li><strong>Nombre de la Agencia:</strong> {agency.Name}</li>
                    <li><strong>Código de la Agencia:</strong> {agency.AgencyCode}</li>
                </ul>
                <p>Ya puede acceder a la información de esta agencia a través de su cuenta en el sistema.</p>
                <p>Si tiene alguna pregunta o necesita asistencia, no dude en contactarnos.</p>
                <p>Atentamente,<br>El equipo de NUTRE</p>
            ";

            string recipientEmail = user.Email;
            string recipientName = $"{user.FirstName} {user.FatherLastName}";

#if DEBUG || LOCAL
            // En modo DEBUG o LOCAL, usar la dirección de desarrollo si está configurada
            if (!string.IsNullOrEmpty(_appSettings.Gmail.EmailToDev))
            {
                recipientEmail = _appSettings.Gmail.EmailToDev;
                _logger.LogInformation($"Modo DEBUG/LOCAL: Enviando correo a {recipientEmail} en lugar de {user.Email}");
            }
#endif

            await SendEmailWithGmailAsync(recipientEmail, subject, body);
            _logger.LogInformation($"Correo de asignación de agencia enviado exitosamente a {recipientEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al enviar correo de asignación de agencia a {user.Email}");
            throw; // Re-throw the exception to be handled by the caller
        }
    }

    public async Task SendAgencyUnassignmentEmail(DTOUser user, DTOAgency agency)
    {
        var subject = "Desasignación de Agencia";
        var body = $"Estimado/a {user.FirstName} {user.FatherLastName},\n\n" +
                  $"Le informamos que ha sido desasignado/a como monitor de la agencia {agency.Name}.\n\n" +
                  "Gracias por su atención.";

        string recipientEmail = user.Email;
        string recipientName = $"{user.FirstName} {user.FatherLastName}";

#if DEBUG || LOCAL
        // En modo DEBUG o LOCAL, usar la dirección de desarrollo si está configurada
        if (!string.IsNullOrEmpty(_appSettings.Gmail.EmailToDev))
        {
            recipientEmail = _appSettings.Gmail.EmailToDev;
            _logger.LogInformation($"Modo DEBUG/LOCAL: Enviando correo a {recipientEmail} en lugar de {user.Email}");
        }
#endif

        await SendEmailWithGmailAsync(recipientEmail, subject, body);
        _logger.LogInformation($"Correo de desasignación de agencia enviado exitosamente a {recipientEmail}");
    }
}
