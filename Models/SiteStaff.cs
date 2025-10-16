namespace Api.Models;

/// <summary>
/// Modelo que representa la relación entre un sitio (Site) y un empleado del staff
/// </summary>
public class SiteStaff
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
}
