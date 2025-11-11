using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para SiteProgram
/// </summary>
public class SiteProgramResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public int ProgramId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Información del programa relacionado
    public string? ProgramName { get; set; }
    public string? ProgramNameEN { get; set; }
}

