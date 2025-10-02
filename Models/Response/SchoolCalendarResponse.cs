namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para el calendario de funcionamiento de una escuela
/// Contiene información de la escuela y sus días de funcionamiento
/// </summary>
public class SchoolCalendarResponse
{
    /// <summary>
    /// ID de la escuela
    /// </summary>
    public int schoolId { get; set; }

    /// <summary>
    /// Nombre de la escuela
    /// </summary>
    public string schoolName { get; set; } = string.Empty;

    /// <summary>
    /// Lista de días de funcionamiento de la escuela
    /// </summary>
    public List<OperatingDayResponse> operatingDays { get; set; } = new();
}
