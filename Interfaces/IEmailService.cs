using Api.Models;
using Api.Models.Response;

namespace Api.Interfaces;

public interface IEmailService
{
    Task SendEmail(string email, string subject, string message);
    Task SendTemporaryPasswordEmail(string email, string temporaryPassword);
    Task SendEmailWithGmail(string email, string subject, string message, string emailType = "Generic", string? userId = null, int? agencyId = null, string? emailTemplateKey = null);
    Task SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword, string? userId = null);
    Task SendApprovalSponsorEmail(User userRequest, string temporaryPassword, string? fullName = null);
    Task SendDenialSponsorEmail(string email, string fullName, string rejectionReason);
    Task SendAgencyAssignmentEmail(DTOUser user, AgencyResponse agency);
    Task SendAgencyUnassignmentEmail(DTOUser user, AgencyResponse agency);
    Task SendPasswordChangedEmail(DTOUser user, string newPassword);
    Task SendPasswordResetEmail(string email, string resetLink);
    Task<bool> ResendEmail(int emailLogId, bool forceResend = false);
    /// <summary>Notifica a administradores sobre una solicitud de extensión de rol secundario.</summary>
    Task SendRoleExtensionRequestToAdmins(IEnumerable<string> adminEmails, string userName, string userEmail, string roleName, DateTime requestedValidTo, string? reason);
    /// <summary>Notifica al usuario que su solicitud de extensión fue aprobada.</summary>
    Task SendRoleExtensionApprovedEmail(string userEmail, string userName, string roleName, DateTime newValidTo);
    /// <summary>Notifica al usuario que su solicitud de extensión fue rechazada.</summary>
    Task SendRoleExtensionRejectedEmail(string userEmail, string userName, string roleName);
}
