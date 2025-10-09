using System;

namespace Api.Models;

/// <summary>
/// DTO para la relación entre un sitio (Site) y un empleado del staff
/// </summary>
public class DTOSiteStaff
{
    public int Id { get; set; }

    // Relaciones principales
    public int SiteId { get; set; }
    public int StaffId { get; set; }

    // Información de la asignación
    public DateTime AssignmentDate { get; set; }
    public int AssignmentTypeId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }

    // Estado y auditoría
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Información relacionada para consultas
    public string? SiteName { get; set; }
    public string? StaffName { get; set; }
    public string? AssignmentTypeName { get; set; }
}
