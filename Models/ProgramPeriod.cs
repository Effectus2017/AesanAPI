using System;

namespace Api.Models;

/// <summary>
/// Modelo de entidad ProgramPeriod
/// Representa un período de fechas para un programa en un año específico
/// </summary>
public class ProgramPeriod
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

