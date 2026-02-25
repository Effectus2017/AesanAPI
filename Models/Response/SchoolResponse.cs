using System;
using Api.Models;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para School con datos relacionados
/// </summary>
public class SchoolResponse
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SchoolCode { get; set; }
    public int SchoolNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int SitesCount { get; set; }

    // Objetos relacionados
    public DTOAgency? Agency { get; set; }

    // Propiedades de conveniencia para operaciones que necesiten IDs
    public string? AgencyName => Agency?.Name;
    public string? AgencyCode => Agency?.AgencyCode;
}
