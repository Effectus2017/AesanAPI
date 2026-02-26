namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para el calendario de citas de una agencia
/// Contiene información de la agencia y sus citas
/// </summary>
public class AgencyCalendarResponse
{
    /// <summary>
    /// ID de la agencia
    /// </summary>
    public int AgencyId { get; set; }

    /// <summary>
    /// Nombre de la agencia
    /// </summary>
    public string AgencyName { get; set; } = string.Empty;

    /// <summary>
    /// Lista de citas
    /// </summary>
    public List<AgencyAppointmentResponse> Appointments { get; set; } = new();
}
