using Api.Models;

namespace Api.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendTemporaryPasswordEmail(string email, string temporaryPassword);
    Task SendEmailWithGmailAsync(string email, string subject, string message);
    Task SendWelcomeAgencyEmail(UserAgencyRequest userRequest, string temporaryPassword);
    Task SendApprovalSponsorEmail(User userRequest, string temporaryPassword);
    Task SendDenialSponsorEmail(User user, string rejectionReason);
    Task SendAgencyAssignmentEmail(DTOUser user, DTOAgency agency);
    Task SendAgencyUnassignmentEmail(DTOUser user, DTOAgency agency);
    Task SendPasswordChangedEmail(DTOUser user, string newPassword);
}
