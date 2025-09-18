using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para SchoolSatellite
/// Contiene los datos necesarios para crear o actualizar una relación escuela-satélite
/// </summary>
public class SchoolSatelliteRequest
{
    public int Id { get; set; }
    public int MainSchoolId { get; set; }
    public int SatelliteSchoolId { get; set; }
    public DateTime? AssignmentDate { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
}
