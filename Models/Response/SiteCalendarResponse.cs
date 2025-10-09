namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para el calendario de funcionamiento de un sitio
/// Contiene información del sitio y sus días de funcionamiento
/// </summary>
public class SiteCalendarResponse
{
    /// <summary>
    /// ID del sitio
    /// </summary>
    public int SiteId { get; set; }

    /// <summary>
    /// Nombre del sitio
    /// </summary>
    public string SiteName { get; set; } = string.Empty;

    /// <summary>
    /// Lista de días de funcionamiento del sitio
    /// </summary>
    public List<OperatingDayResponse> OperatingDays { get; set; } = new();
}
