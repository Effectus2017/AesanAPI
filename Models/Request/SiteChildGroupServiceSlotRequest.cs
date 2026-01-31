namespace Api.Models.Request;

public class SiteChildGroupServiceSlotRequest
{
    public int? Id { get; set; }
    public int ServiceTypeId { get; set; }
    public bool IsOffered { get; set; }

    /// <summary>Hora inicio. El frontend envía "fromTime" (camelCase).</summary>
    public TimeSpan? FromTime { get; set; }

    /// <summary>Hora fin. El frontend envía "toTime" (camelCase).</summary>
    public TimeSpan? ToTime { get; set; }
}
