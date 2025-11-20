using System;
using System.Collections.Generic;

namespace Api.Models.Request;

public class SiteExcursionRequest
{
    public int? Id { get; set; }
    public int SiteId { get; set; }
    public int? ChildGroupId { get; set; }
    public required string ActivityDescription { get; set; }
    public DateTime ExcursionDate { get; set; }
    public bool IsFullDay { get; set; }
    public bool IsUnforeseen { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;
    public List<int> ExcludedServiceTypeIds { get; set; } = new();
}

