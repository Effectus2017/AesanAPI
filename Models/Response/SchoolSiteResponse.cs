using System;
using Api.Models;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para SchoolSite con datos relacionados
/// </summary>
public class SchoolSiteResponse
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int SiteId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Objetos relacionados
    public SchoolResponse? School { get; set; }
    public SiteResponse? Site { get; set; }

    // Propiedades de conveniencia
    public string? SchoolName => School?.Name;
    public string? SiteName => Site?.Name;
}
