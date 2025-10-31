using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para la relación entre un sitio (Site) y un empleado del staff
/// </summary>
public class SiteStaffResponse
{
    public int Id { get; set; }

    // Relaciones principales
    public int SiteId { get; set; }
    public int StaffId { get; set; }

    // Información de la asignación
    public DateTime AssignmentDate { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }

    // Estado y auditoría
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Información relacionada para consultas (GetStaffBySite)
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? FatherLastName { get; set; }
    public string? MotherLastName { get; set; }
    public string? Email { get; set; }

    // Información relacionada para consultas (GetSitesByStaff)
    public string? SiteName { get; set; }
    public string? SiteAddress { get; set; }
    public int? SiteCityId { get; set; }
    public int? SiteRegionId { get; set; }
    public string? SiteZipCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? AgencyName { get; set; }
    public bool? AgencyIsActive { get; set; }
}

