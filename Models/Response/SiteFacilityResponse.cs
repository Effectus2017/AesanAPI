namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para instalaciones de sitios
/// </summary>
public class SiteFacilityResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public int FacilityTypeId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Objetos relacionados
    public string? FacilityTypeName { get; set; }
    public string? FacilityTypeNameEN { get; set; }
    public string? FacilityTypeOptionKey { get; set; }
}
