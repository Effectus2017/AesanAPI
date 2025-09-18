using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para SchoolSatellite
/// Contiene información sobre la relación entre una escuela principal y sus satélites
/// </summary>
public class SchoolSatelliteResponse
{
    public int Id { get; set; }
    public int MainSchoolId { get; set; }
    public int SatelliteSchoolId { get; set; }
    public string SatelliteSchoolName { get; set; }
    public DateTime? AssignmentDate { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
