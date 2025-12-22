namespace Api.Models;

/// <summary>
/// Modelo que define qué días de la semana puede operar cada programa
/// </summary>
public class ProgramOperatingDays
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
    public int DayOfWeek { get; set; } // 1=Lunes, 2=Martes, ..., 7=Domingo
    public bool IsAllowed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

