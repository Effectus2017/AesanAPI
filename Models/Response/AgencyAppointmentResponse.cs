namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para una cita de una agencia
/// </summary>
public class AgencyAppointmentResponse
{
    /// <summary>
    /// ID de la cita
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del agencia
    /// </summary>
    public int AgencyId { get; set; }
    
    /// <summary>
    /// Fecha de la cita
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Hora de inicio
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// Hora de fin
    /// </summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>
    /// Comentario
    /// </summary>
    public string? Comment { get; set; }
}
