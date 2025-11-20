using Api.Interfaces;
using Api.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MimeKit.Text;

namespace Api.Services;


public class EmailService(IOptions<ApplicationSettings> appSettings, ILogger<EmailService> logger, IWebHostEnvironment environment) : IEmailService
{
    private readonly ApplicationSettings _appSettings = appSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;
    private readonly IWebHostEnvironment _environment = environment;
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        await SendEmailWithGmailAsync(email, subject, message);
    }

    public async Task SendTemporaryPasswordEmail(string email, string temporaryPassword)
    {
        var subject = "Tu contraseña temporera";
        var message = $"Tu contraseña temporera es: {temporaryPassword}";
        await SendEmailWithGmailAsync(email, subject, message);
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
    /// Obtiene la URL del Web según el ambiente actual
    /// </summary>
    private string GetWebUrl()
    {
        string webUrl = _environment.EnvironmentName.ToLower() switch
        {
            "development" => _appSettings.LocalWebURL,
            "staging" => _appSettings.StagingWebURL,
            "production" => _appSettings.ProduccionWebURL,
            _ => _appSettings.LocalWebURL
        };

        // Asegurar que la URL termina con '/'
        if (!string.IsNullOrEmpty(webUrl) && !webUrl.EndsWith("/"))
        {
            webUrl += "/";
        }

        return webUrl ?? _appSettings.LocalWebURL;
    }

    /// <summary>
    /// Envía un correo de bienvenida al auspiciador de una agencia
    /// </summary>
    /// <param name="userRequest">Datos del usuario y la agencia</param>
    /// <param name="temporaryPassword">Contraseña temporera asignada</param>
    public async Task SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword)
    {
        _logger.LogInformation("Enviando correo de bienvenida a la agencia");

        var subject = "¡Gracias por su interés en formar parte del programa de AESAN!";
        var fullName = $"{userRequest.Staff.FirstName} {userRequest.Staff.FatherLastName}";
        var webUrl = GetWebUrl();

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <p>Estimado/a {fullName},</p>
                
                <p>Nos complace enormemente saber que está interesado/a en formar parte de nuestros programas en AESAN. 
                Su apoyo y participación son fundamentales para continuar con nuestra misión de ofrecer servicios de alimentos 
                y educación nutricional a los participantes, cubriendo todas las edades desde la niñez hasta la vejez.</p>
                
                <p>Para continuar con su proceso de registro, validación y aprobación final, le pedimos que siga los siguientes pasos:</p>
                
                <ul>
                    <li>Haz clic en el siguiente enlace para acceder a la plataforma <strong>NUTRE</strong>: <a href='{webUrl}' style='color: #0066cc; text-decoration: none; font-weight: bold;'>{webUrl}</a></li>
                    <li>Ingrese su correo electrónico como nombre de usuario: <strong>{userRequest.Staff.Email}</strong></li>
                    <li>Luego coloque la contraseña temporera <strong>{temporaryPassword}</strong></li>
                    <li>Deberá completar la sección en el menú principal llamada <strong>""Sitios""</strong>. Aquí deberá incluir todos los Sitios asociados a su Organización o Institución que estarán participando del programa de su interés.</li>
                    <li>Deberá completar la sección en el menú principal llamada <strong>""Personal""</strong> donde listará todos los empleados administrativos y operacionales, así como los miembros de la junta directiva.</li>
                </ul>
                
                <p>Si tiene alguna pregunta o necesita asistencia adicional, no dude en contactarnos. 
                Estamos aquí para ayudarle en cada paso del camino.</p>
                
                <p>Una vez más, gracias por su interés y confianza en AESAN.<br>
                Juntos podemos lograr grandes cosas.</p>
                
                <p>Saludos cordiales,<br>
                <strong>Agencia Estatal Servicios de Alimentos y Nutrición</strong><br>
                (787) 759-2000 / Exts. 4625751, 4625753</p>
            </div>";

        await SendEmailWithGmailAsync(userRequest.Staff.Email, subject, htmlBody);
    }

    /// <summary>
    /// Envía un correo de confirmación de aprobación de auspiciador
    /// </summary>
    /// <param name="user">Usuario al que se le envía el correo</param>
    /// <param name="temporaryPassword">Contraseña temporera asignada</param>
    /// <param name="fullName">Nombre completo del usuario (opcional)</param>
    public async Task SendApprovalSponsorEmail(User user, string temporaryPassword, string? fullName = null)
    {
        _logger.LogInformation("Enviando correo de confirmación de aprobación de auspiciador");

        var subject = "¡Gracias por su interés en formar parte del programa de AESAN!";
        // Usar el nombre completo proporcionado, o "Usuario" como fallback
        var userName = !string.IsNullOrWhiteSpace(fullName) ? fullName : "Usuario";
        var webUrl = GetWebUrl();

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <p>Estimado/a {userName},</p>
                
                <p>Nos complace enormemente saber que está interesado/a en formar parte de nuestros programas en AESAN. 
                Su apoyo y participación son fundamentales para continuar con nuestra misión de ofrecer servicios de alimentos 
                y educación nutricional a los participantes, cubriendo todas las edades desde la niñez hasta la vejez.</p>
                
                <p>Para continuar con su proceso de registro, validación y aprobación final, le pedimos que siga los siguientes pasos:</p>
                
                <ul>
                    <li>Haz clic en el siguiente enlace para acceder a la plataforma <strong>NUTRE</strong>: <a href='{webUrl}' style='color: #0066cc; text-decoration: none; font-weight: bold;'>{webUrl}</a></li>
                    <li>Luego coloque la contraseña temporera <strong>{temporaryPassword}</strong></li>
                    <li>Deberá completar la sección en el menú principal llamada <strong>""Sitios""</strong>. Aquí deberá incluir todos los Sitios asociados a su Organización o Institución que estarán participando del programa de su interés.</li>
                    <li>Deberá completar la sección en el menú principal llamada <strong>""Personal""</strong> donde listará todos los empleados administrativos y operacionales, así como los miembros de la junta directiva.</li>
                </ul>
                
                <p>Si tiene alguna pregunta o necesita asistencia adicional, no dude en contactarnos. 
                Estamos aquí para ayudarle en cada paso del camino.</p>
                
                <p>Una vez más, gracias por su interés y confianza en AESAN.<br>
                Juntos podemos lograr grandes cosas.</p>
                
                <p>Saludos cordiales,<br>
                <strong>Agencia Estatal Servicios de Alimentos y Nutrición</strong><br>
                (787) 759-2000 / Exts. 4625751, 4625753</p>
            </div>";

#if !DEBUG
        await SendEmailWithGmailAsync(user.Email, subject, htmlBody);
#endif

    }

    /// <summary>
    /// Envía un correo de confirmación de denegación de auspiciador
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <param name="fullName">Nombre completo del usuario</param>
    /// <param name="rejectionReason">Razón del rechazo</param>
    public async Task SendDenialSponsorEmail(string email, string fullName, string rejectionReason)
    {
        _logger.LogInformation("Enviando correo de confirmación de denegación de auspiciador");

        var subject = "Actualización sobre tu aplicación al programa de AESAN";

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <p>Estimado/a {fullName},</p>
                
                <p>Lamentamos informarte que tu aplicación para participar en el programa de AESAN ha sido rechazada por la siguiente razón: {rejectionReason}</p>
                
                <p>Si tienes alguna pregunta o necesitas aclaraciones adicionales, no dudes en contactarnos. 
                Estamos aquí para ayudarte y proporcionar más información si lo necesitas.</p>
                
                <p>Agradecemos sinceramente tu interés en AESAN y te deseamos éxito en tus futuros proyectos.</p>
                
                <p>Atentamente,<br>
                Equipo de AESAN<br>
                {email}<br>
                </p>
            </div>";

#if !DEBUG
        await SendEmailWithGmailAsync(email, subject, htmlBody);
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

    /// <summary>
    /// Envía un correo electrónico notificando la desasignación de una agencia a un usuario
    /// </summary>
    /// <param name="user">Usuario al que se le desasignó la agencia</param>
    /// <param name="agency">Agencia desasignada</param>
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


    /// <summary>
    /// Envía un correo electrónico notificando el cambio de contraseña de un usuario
    /// </summary>
    /// <param name="user">Usuario al que se le cambió la contraseña</param>
    /// <param name="newPassword">Nueva contraseña temporera</param>
    public async Task SendPasswordChangedEmail(DTOUser user, string newPassword)
    {
        var subject = "Tu contraseña temporera ha sido generada";
        var fullName = $"{user.FirstName} {user.FatherLastName}";
        var plainTextContent = $"Un administrador ha generado una contraseña temporera para tu cuenta. Tu contraseña temporera es: {newPassword}. Al ingresar con esta contraseña, el sistema te guiará para crear una nueva contraseña segura.";

        var htmlContent = $@"
            <h2>Se ha generado una contraseña temporera para tu cuenta</h2>
            <p>Estimado/a {fullName},</p>
            <p>Un administrador ha generado una contraseña temporera para tu cuenta en el sistema.</p>
            <p>Tu contraseña temporera es: <strong>{newPassword}</strong></p>
            <p><strong>Importante:</strong></p>
            <ul>
                <li>Esta es una contraseña temporera que debes cambiar en tu próximo inicio de sesión.</li>
                <li>Al ingresar con esta contraseña, el sistema te guiará automáticamente para crear una nueva contraseña segura.</li>
                <li>Por razones de seguridad, no compartas esta contraseña con nadie.</li>
            </ul>
            <p>Si no has solicitado este cambio o tienes alguna pregunta, por favor contacta al administrador del sistema.</p>
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

        await SendEmailWithGmailAsync(recipientEmail, subject, htmlContent);
    }

    public async Task SendPasswordResetEmail(string email, string resetLink)
    {
        try
        {
            var subject = "Restablecimiento de Contraseña - AESAN";
            var htmlContent = $@"
                <h2>Solicitud de Restablecimiento de Contraseña</h2>
                <p>Has solicitado restablecer tu contraseña en el sistema AESAN.</p>
                <p>Para continuar con el proceso, haz clic en el siguiente enlace:</p>
                <p><a href='{resetLink}'>Restablecer Contraseña</a></p>
                <p><strong>Importante:</strong></p>
                <ul>
                    <li>Este enlace expirará en 30 minutos por razones de seguridad.</li>
                    <li>Si no has solicitado este cambio, puedes ignorar este correo.</li>
                    <li>Tu contraseña actual seguirá siendo válida hasta que completes el proceso de restablecimiento.</li>
                </ul>
                <p>Si tienes alguna pregunta, por favor contacta al administrador del sistema.</p>
            ";

#if DEBUG || LOCAL
            // En modo DEBUG o LOCAL, usar la dirección de desarrollo si está configurada
            if (!string.IsNullOrEmpty(_appSettings.Gmail.EmailToDev))
            {
                email = _appSettings.Gmail.EmailToDev;
                _logger.LogInformation($"Modo DEBUG/LOCAL: Enviando correo a {email}");
            }
#endif

            await SendEmailWithGmailAsync(email, subject, htmlContent);
            _logger.LogInformation("Correo de restablecimiento de contraseña enviado a: {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo de restablecimiento de contraseña a: {Email}", email);
            throw;
        }
    }
}
