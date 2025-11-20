using System;

namespace Api.Models.Response;

public class SiteExcursionExcludedServiceResponse
{
    public int Id { get; set; }
    public int SiteExcursionId { get; set; }
    public int ServiceTypeId { get; set; }
    public string? ServiceTypeName { get; set; }
    public string? ServiceTypeNameEN { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

