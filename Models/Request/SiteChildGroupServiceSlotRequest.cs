using System;

namespace Api.Models.Request;

public class SiteChildGroupServiceSlotRequest
{
    public int? Id { get; set; }
    public int ServiceTypeId { get; set; }
    public bool IsOffered { get; set; }
    public TimeSpan? FromTime { get; set; }
    public TimeSpan? ToTime { get; set; }
}
