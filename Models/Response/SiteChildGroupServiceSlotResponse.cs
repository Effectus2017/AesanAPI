using System;

namespace Api.Models.Response;

public class SiteChildGroupServiceSlotResponse
{
    public int Id { get; set; }
    public int ChildGroupId { get; set; }
    public int ServiceTypeId { get; set; }
    public bool IsOffered { get; set; }
    public TimeSpan? FromTime { get; set; }
    public TimeSpan? ToTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ServiceTypeName { get; set; }
    public string? ServiceTypeNameEN { get; set; }
}
