namespace Api.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendTemporaryPasswordEmail(string email, string temporaryPassword);
}
