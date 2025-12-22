namespace Api.Models;

/// <summary>
/// Modelo de respuesta para los días de la semana con sus nombres
/// </summary>
public class DayOfWeekResponse
{
    /// <summary>
    /// ID del día de la semana (1=Lunes, 2=Martes, ..., 7=Domingo)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del día en español
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del día en inglés
    /// </summary>
    public string NameEN { get; set; } = string.Empty;
}

