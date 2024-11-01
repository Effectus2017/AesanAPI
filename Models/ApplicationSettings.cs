namespace Api.Models;

public class ApplicationSettings
{
    public string Secret { get; set; } = "";
    public string EmailFrom { get; set; } = "";
    public string EmailContacto { get; set; } = "";
    public string ConfirmationMailMessage { get; set; } = "";
    public string ConfirmationMailMessageSendGrid { get; set; } = "";
    public string VerificationMailMessage { get; set; } = "";
    public string VerificationMailMessageSendGrid { get; set; } = "";
    public string CompleteMailMessage { get; set; } = "";
    public string NewPasswordMailMessage { get; set; } = "";
    public string ResetPasswordMailMessage { get; set; } = "";

    public string LocalURL { get; set; } = "";
    public string StagingURL { get; set; } = "";
    public string ProduccionURL { get; set; } = "";

    public string LocalWebURL { get; set; } = "";
    public string StagingWebURL { get; set; } = "";
    public string ProduccionWebURL { get; set; } = "";
    // Gmail settings
    public GmailSettings Gmail { get; set; } = new GmailSettings();
}

public class GmailSettings
{
    public string EmailFrom { get; set; } = "";
    public string SmtpServer { get; set; } = "";
    public int SmtpServerPort { get; set; } = 0;
    public string SmtpUser { get; set; } = "";
    public string SmtpPass { get; set; } = "";
}
