using System;

namespace Api.Models;

/// <summary>
/// Modelo de entidad School (Escuela)
/// Representa una escuela que puede tener múltiples sitios asignados
/// </summary>
public class School
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SchoolCode { get; set; }
    public int SchoolNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
