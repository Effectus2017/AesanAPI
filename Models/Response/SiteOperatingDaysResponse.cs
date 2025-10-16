namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para días de funcionamiento de sitios
/// </summary>
public class SiteOperatingDaysResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public DateTime OperatingDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Información del sitio
    public string? SiteName { get; set; }
}
