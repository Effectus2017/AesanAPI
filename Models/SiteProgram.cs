using System;

namespace Api.Models;

/// <summary>
/// Modelo de entidad SiteProgram
/// Representa la relación entre un Site y un Program con fechas variables
/// </summary>
public class SiteProgram
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public int ProgramId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

