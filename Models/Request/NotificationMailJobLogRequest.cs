namespace Api.Models.Request;

/// <summary>
/// Request body for POST /notifications/job-log (uso interno / Web Job).
/// </summary>
public class NotificationMailJobLogRequest
{
    public string JobName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}
