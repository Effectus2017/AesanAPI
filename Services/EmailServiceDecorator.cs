using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Microsoft.Extensions.Logging;

namespace Api.Services;

/// <summary>
/// Decorator que envuelve EmailService y registra automáticamente todos los envíos de correo en EmailLog
/// </summary>
public class EmailServiceDecorator : IEmailService
{
    private readonly IEmailService _emailService;
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly ILogger<EmailServiceDecorator> _logger;

    public EmailServiceDecorator(
        IEmailService emailService,
        IEmailLogRepository emailLogRepository,
        ILogger<EmailServiceDecorator> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _emailLogRepository = emailLogRepository ?? throw new ArgumentNullException(nameof(emailLogRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendEmail(string email, string subject, string message)
    {
        await LogAndSendEmail(
            email: email,
            subject: subject,
            message: message,
            emailType: "Generic",
            userId: null,
            agencyId: null,
            emailTemplateKey: null,
            sendAction: () => _emailService.SendEmail(email, subject, message)
        );
    }

    public async Task SendTemporaryPasswordEmail(string email, string temporaryPassword)
    {
        await LogAndSendEmail(
            email: email,
            subject: "Contraseña Temporal",
            message: $"Su contraseña temporal es: {temporaryPassword}",
            emailType: "TemporaryPassword",
            userId: null,
            agencyId: null,
            emailTemplateKey: "TemporaryPassword",
            sendAction: () => _emailService.SendTemporaryPasswordEmail(email, temporaryPassword)
        );
    }

    public async Task SendEmailWithGmail(string email, string subject, string message, string emailType = "Generic", string? userId = null, int? agencyId = null, string? emailTemplateKey = null)
    {
        await LogAndSendEmail(
            email: email,
            subject: subject,
            message: message,
            emailType: emailType,
            userId: userId,
            agencyId: agencyId,
            emailTemplateKey: emailTemplateKey,
            sendAction: () => _emailService.SendEmailWithGmail(email, subject, message, emailType, userId, agencyId, emailTemplateKey)
        );
    }

    public async Task SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword, string? userId = null)
    {
        await LogAndSendEmail(
            email: userRequest.Staff.Email,
            subject: "Bienvenida a AESAN",
            message: $"Bienvenido {userRequest.Staff.FirstName}",
            emailType: "WelcomeAgency",
            userId: userId,
            agencyId: null,
            emailTemplateKey: "WelcomeAgency",
            sendAction: () => _emailService.SendWelcomeAgencyEmail(userRequest, temporaryPassword, userId)
        );
    }

    public async Task SendApprovalSponsorEmail(User userRequest, string temporaryPassword, string? fullName = null)
    {
        await LogAndSendEmail(
            email: userRequest.Email ?? "",
            subject: "Aprobación de Auspiciador",
            message: "Su solicitud ha sido aprobada",
            emailType: "ApprovalSponsor",
            userId: userRequest.Id,
            agencyId: null,
            emailTemplateKey: "ApprovalSponsor",
            sendAction: () => _emailService.SendApprovalSponsorEmail(userRequest, temporaryPassword, fullName)
        );
    }

    public async Task SendDenialSponsorEmail(string email, string fullName, string rejectionReason)
    {
        await LogAndSendEmail(
            email: email,
            subject: "Denegación de Auspiciador",
            message: rejectionReason,
            emailType: "DenialSponsor",
            userId: null,
            agencyId: null,
            emailTemplateKey: "DenialSponsor",
            sendAction: () => _emailService.SendDenialSponsorEmail(email, fullName, rejectionReason)
        );
    }

    public async Task SendAgencyAssignmentEmail(DTOUser user, DTOAgency agency)
    {
        await LogAndSendEmail(
            email: user.Email ?? "",
            subject: "Asignación de Agencia",
            message: $"Ha sido asignado a la agencia {agency.Name}",
            emailType: "AgencyAssignment",
            userId: user.Id,
            agencyId: agency.Id,
            emailTemplateKey: "AgencyAssignment",
            sendAction: () => _emailService.SendAgencyAssignmentEmail(user, agency)
        );
    }

    public async Task SendAgencyUnassignmentEmail(DTOUser user, DTOAgency agency)
    {
        await LogAndSendEmail(
            email: user.Email ?? "",
            subject: "Desasignación de Agencia",
            message: $"Ha sido desasignado de la agencia {agency.Name}",
            emailType: "AgencyUnassignment",
            userId: user.Id,
            agencyId: agency.Id,
            emailTemplateKey: "AgencyUnassignment",
            sendAction: () => _emailService.SendAgencyUnassignmentEmail(user, agency)
        );
    }

    public async Task SendPasswordChangedEmail(DTOUser user, string newPassword)
    {
        await LogAndSendEmail(
            email: user.Email ?? "",
            subject: "Contraseña Cambiada",
            message: "Su contraseña ha sido cambiada",
            emailType: "PasswordChanged",
            userId: user.Id,
            agencyId: null,
            emailTemplateKey: "PasswordChanged",
            sendAction: () => _emailService.SendPasswordChangedEmail(user, newPassword)
        );
    }

    public async Task SendPasswordResetEmail(string email, string resetLink)
    {
        await LogAndSendEmail(
            email: email,
            subject: "Restablecimiento de Contraseña",
            message: $"Link: {resetLink}",
            emailType: "PasswordReset",
            userId: null,
            agencyId: null,
            emailTemplateKey: "PasswordReset",
            sendAction: () => _emailService.SendPasswordResetEmail(email, resetLink)
        );
    }

    public async Task<bool> ResendEmail(int emailLogId, bool forceResend = false)
    {
        // Para reenvío, obtener el log existente y crear uno nuevo
        try
        {
            var existingLog = await _emailLogRepository.GetEmailLogById(emailLogId);
            if (existingLog == null)
            {
                _logger.LogWarning("No se encontró el log de correo con ID {EmailLogId} para reenvío", emailLogId);
                return false;
            }

            // Crear nuevo log para el reenvío
            var logRequest = new EmailLogRequest
            {
                RecipientEmail = existingLog.RecipientEmail,
                Subject = existingLog.Subject,
                EmailType = existingLog.EmailType,
                Status = "Pending",
                AttemptedAt = DateTime.Now,
                UserId = existingLog.UserId,
                AgencyId = existingLog.AgencyId,
                EmailTemplateKey = existingLog.EmailTemplateKey
            };

            var newLogId = await _emailLogRepository.InsertEmailLog(logRequest);
            _logger.LogInformation("Nuevo log creado para reenvío con ID {NewLogId} basado en log {OriginalLogId}", newLogId, emailLogId);

            // Ejecutar el reenvío
            var result = await _emailService.ResendEmail(emailLogId, forceResend);

            // Actualizar el nuevo log según el resultado
            if (result)
            {
                await _emailLogRepository.UpdateEmailLogStatus(newLogId, "Sent");
            }
            else
            {
                await _emailLogRepository.UpdateEmailLogStatus(newLogId, "Failed", "Error al reenviar correo");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar reenvío de correo con ID {EmailLogId}", emailLogId);
            // Continuar con el reenvío aunque falle el logging
            return await _emailService.ResendEmail(emailLogId, forceResend);
        }
    }

    /// <summary>
    /// Método helper que registra el envío de correo antes y después de ejecutarlo
    /// </summary>
    private async Task LogAndSendEmail(
        string email,
        string subject,
        string message,
        string emailType,
        string? userId,
        int? agencyId,
        string? emailTemplateKey,
        Func<Task> sendAction)
    {
        int emailLogId = 0;

        // Crear log con estado 'Pending' antes del envío
        try
        {
            var logRequest = new EmailLogRequest
            {
                RecipientEmail = email,
                Subject = subject,
                EmailType = emailType,
                Status = "Pending",
                AttemptedAt = DateTime.Now,
                UserId = userId,
                AgencyId = agencyId,
                EmailTemplateKey = emailTemplateKey
            };

            emailLogId = await _emailLogRepository.InsertEmailLog(logRequest);
            _logger.LogInformation("Log de correo creado con ID {EmailLogId} para {Email}", emailLogId, email);
        }
        catch (Exception logEx)
        {
            _logger.LogError(logEx, "Error al crear log de correo (continuando con el envío)");
            // Continuar con el envío aunque falle el logging
        }

        // Ejecutar el envío
        try
        {
            await sendAction();

            // Actualizar log a 'Sent' si fue exitoso
            if (emailLogId > 0)
            {
                try
                {
                    await _emailLogRepository.UpdateEmailLogStatus(emailLogId, "Sent");
                    _logger.LogInformation("Log de correo actualizado a 'Sent' para ID {EmailLogId}", emailLogId);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error al actualizar log de correo a 'Sent' (no crítico)");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo electrónico a {Email}. Detalles: {Message}", email, ex.Message);

            // Actualizar log a 'Failed' con mensaje de error
            if (emailLogId > 0)
            {
                try
                {
                    await _emailLogRepository.UpdateEmailLogStatus(emailLogId, "Failed", ex.Message);
                    _logger.LogInformation("Log de correo actualizado a 'Failed' para ID {EmailLogId}", emailLogId);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error al actualizar log de correo a 'Failed' (no crítico)");
                }
            }

            // Re-lanzar la excepción para que el código que llama pueda manejarla
            throw;
        }
    }
}
