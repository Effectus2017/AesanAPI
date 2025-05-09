using System;
using Api.Models;

public class DTOSchoolFacility
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int FacilityId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public DTOFacility Facility { get; set; }
}