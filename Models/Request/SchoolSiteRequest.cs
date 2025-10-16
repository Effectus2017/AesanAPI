using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para operaciones con SchoolSite
/// </summary>
public class SchoolSiteRequest
{
    public int? Id { get; set; }
    public int SchoolId { get; set; }
    public int SiteId { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;
}
