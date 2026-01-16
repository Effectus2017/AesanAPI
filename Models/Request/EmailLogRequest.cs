namespace Api.Models.Request;

public class EmailLogRequest
{
    public string RecipientEmail { get; set; } = "";
    public string Subject { get; set; } = "";
    public string EmailType { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? AttemptedAt { get; set; }
    public string? UserId { get; set; }
    public int? AgencyId { get; set; }
    public string? EmailTemplateKey { get; set; }
    public int RetryCount { get; set; } = 0;
    public int? OriginalEmailLogId { get; set; }
    public string? CreatedBy { get; set; }
}

public class ResendEmailRequest
{
    public int EmailLogId { get; set; }
    public bool ForceResend { get; set; } = false;
}
