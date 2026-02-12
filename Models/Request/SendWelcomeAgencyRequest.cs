namespace Api.Models.Request;

/// <summary>
/// Request body for POST /notifications/send-welcome-agency (uso interno / Web Job).
/// </summary>
public class SendWelcomeAgencyRequest
{
    public int AgencyId { get; set; }
}
