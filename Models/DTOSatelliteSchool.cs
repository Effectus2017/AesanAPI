using System;
using Api.Models;

public class DTOSatelliteSchool
{
    public int Id { get; set; }
    public int MainSchoolId { get; set; }
    public int SatelliteSchoolId { get; set; }
    public DateTime? AssignmentDate { get; set; }
    public string Status { get; set; }
    public string Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public DTOSchool MainSchool { get; set; }
    public DTOSchool SatelliteSchool { get; set; }
}