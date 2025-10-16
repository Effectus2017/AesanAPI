namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para información básica de un sitio
/// </summary>
public class SiteInfoResponse
{
    /// <summary>
    /// ID del sitio
    /// </summary>
    public int SiteId { get; set; }

    /// <summary>
    /// Nombre del sitio
    /// </summary>
    public string SiteName { get; set; } = string.Empty;
}
