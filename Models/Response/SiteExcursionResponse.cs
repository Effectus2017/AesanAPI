using System;
using System.Collections.Generic;
using Api.Models.Response;

namespace Api.Models.Response;

public class SiteExcursionResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? AgencyName { get; set; }
    public string? AgencyCode { get; set; }
    public int? ChildGroupId { get; set; }
    public string? ChildGroupName { get; set; }
    public string ActivityDescription { get; set; }
    public DateTime ExcursionDate { get; set; }
    public bool IsFullDay { get; set; }
    public bool IsUnforeseen { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<SiteExcursionExcludedServiceResponse> ExcludedServices { get; set; } = new();
}

