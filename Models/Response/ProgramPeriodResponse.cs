using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para ProgramPeriod
/// </summary>
public class ProgramPeriodResponse
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Información del programa relacionado
    public string? ProgramName { get; set; }
    public string? ProgramNameEN { get; set; }
}

