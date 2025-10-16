using System;

namespace Api.Models;

/// <summary>
/// Modelo de entidad SchoolSite (Relación School-Site)
/// Representa la asignación de un Site a una School
/// </summary>
public class SchoolSite
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int SiteId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
