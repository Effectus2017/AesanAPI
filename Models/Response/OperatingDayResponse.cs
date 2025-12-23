namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para representar un día de funcionamiento de un sitio
/// </summary>
public class OperatingDayResponse
{
    /// <summary>
    /// Identificador único del día de funcionamiento
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del sitio al que pertenece este día de funcionamiento
    /// </summary>
    public int SiteId { get; set; }

    /// <summary>
    /// Fecha de funcionamiento
    /// </summary>
    public DateTime OperatingDate { get; set; }

    /// <summary>
    /// Hora de inicio del funcionamiento (formato HH:MM:SS)
    /// </summary>
    public TimeSpan? StartTime { get; set; }

    /// <summary>
    /// Hora de fin del funcionamiento (formato HH:MM:SS)
    /// </summary>
    public TimeSpan? EndTime { get; set; }

    /// <summary>
    /// Indica si es un fin de semana que funciona por excepción
    /// </summary>
    public bool IsWeekend { get; set; }

    /// <summary>
    /// Indica si el día es feriado
    /// </summary>
    public bool IsHoliday { get; set; }

    /// <summary>
    /// Comentario opcional sobre el día de funcionamiento
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Fecha y hora de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha y hora de la última actualización del registro
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Lista de servicios de alimentación para este día (opcional, se carga bajo demanda)
    /// </summary>
    public List<SiteOperatingDayServiceResponse>? Services { get; set; }
}
