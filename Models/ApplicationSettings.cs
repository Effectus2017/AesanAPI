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

    // Cache settings
    public CacheSettings Cache { get; set; } = new CacheSettings();
    // Gmail settings
    public GmailSettings Gmail { get; set; } = new GmailSettings();
}

public class CacheSettings
{
    public bool Enabled { get; set; } = true;
    public int DefaultExpirationHours { get; set; } = 6;
    public CacheKeys Keys { get; set; } = new();
}

public class CacheKeys
{
    public string Cities { get; set; } = "Cities_{0}_{1}_{2}_{3}";
    public string Regions { get; set; } = "Regions_{0}_{1}_{2}_{3}";
    public string City { get; set; } = "City_{0}";
    public string Region { get; set; } = "Region_{0}";
    public string RegionsByCity { get; set; } = "RegionsByCity_{0}";
    public string CitiesByRegion { get; set; } = "CitiesByRegion_{0}";
}

public class GmailSettings
{
    public string EmailFrom { get; set; } = "";
    public string SmtpServer { get; set; } = "";
    public int SmtpServerPort { get; set; } = 0;
    public string SmtpUser { get; set; } = "";
    public string SmtpPass { get; set; } = "";
    public string EmailToDev { get; set; } = "";
}
