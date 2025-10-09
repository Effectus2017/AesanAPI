using System;

namespace Api.Models.Response;

public class SiteSatelliteResponse
{
    public int Id { get; set; }
    public int MainSiteId { get; set; }
    public int SatelliteSiteId { get; set; }
    public string? SatelliteSiteName { get; set; }
    public DateTime? AssignmentDate { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
