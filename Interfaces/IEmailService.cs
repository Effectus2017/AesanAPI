using Api.Models;

namespace Api.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendTemporaryPasswordEmail(string email, string temporaryPassword);
    Task SendEmailWithGmailAsync(string email, string subject, string message);
    Task SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword, string? userId = null);
    Task SendApprovalSponsorEmail(User userRequest, string temporaryPassword, string? fullName = null);
    Task SendDenialSponsorEmail(string email, string fullName, string rejectionReason);
    Task SendAgencyAssignmentEmail(DTOUser user, DTOAgency agency);
    Task SendAgencyUnassignmentEmail(DTOUser user, DTOAgency agency);
    Task SendPasswordChangedEmail(DTOUser user, string newPassword);
    Task SendPasswordResetEmail(string email, string token);
    Task<bool> ResendEmailAsync(int emailLogId, bool forceResend = false);
}
