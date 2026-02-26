namespace Api.Models.Request;

/// <summary>
/// Modelo de petición para una cita de agencia
/// </summary>
public class AgencyAppointmentRequest
{
    /// <summary>
    /// ID de la cita (opcional, solo para actualización)
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// ID de la agencia
    /// </summary>
    public int AgencyId { get; set; }

    /// <summary>
    /// Fecha de la cita
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Hora de inicio
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// Hora de fin
    /// </summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>
    /// Comentarios
    /// </summary>
    public string? Comment { get; set; }
}
