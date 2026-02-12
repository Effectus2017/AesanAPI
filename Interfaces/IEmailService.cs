using Api.Models;

namespace Api.Interfaces;

public interface IEmailService
{
    Task SendEmail(string email, string subject, string message);
    Task SendTemporaryPasswordEmail(string email, string temporaryPassword);
    Task SendEmailWithGmail(string email, string subject, string message, string emailType = "Generic", string? userId = null, int? agencyId = null, string? emailTemplateKey = null);
    Task SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword, string? userId = null);
    Task SendApprovalSponsorEmail(User userRequest, string temporaryPassword, string? fullName = null);
    Task SendDenialSponsorEmail(string email, string fullName, string rejectionReason);
    Task SendAgencyAssignmentEmail(DTOUser user, DTOAgency agency);
    Task SendAgencyUnassignmentEmail(DTOUser user, DTOAgency agency);
    Task SendPasswordChangedEmail(DTOUser user, string newPassword);
    Task SendPasswordResetEmail(string email, string resetLink);
    Task<bool> ResendEmail(int emailLogId, bool forceResend = false);
}
