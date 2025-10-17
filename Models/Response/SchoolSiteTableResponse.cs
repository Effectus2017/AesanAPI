namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para SchoolSite en tablas/listas
/// Contiene únicamente los datos necesarios para mostrar en tablas sin objetos anidados
/// </summary>
public class SchoolSiteTableResponse
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int SiteId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Campos del Site (planos, no anidados)
    public string SiteName { get; set; } = string.Empty;
    public string? SiteCode { get; set; }
    public int? SiteNumber { get; set; }
    public string Address { get; set; } = string.Empty;
    public bool SiteIsActive { get; set; }
}
