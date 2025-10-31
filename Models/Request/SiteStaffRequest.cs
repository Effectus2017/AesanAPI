namespace Api.Models.Request;

/// <summary>
/// Modelo de request para las operaciones de asignación de staff a sitios
/// </summary>
public class SiteStaffRequest
{
    public int SiteId { get; set; }
    public int StaffId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
}

/// <summary>
/// Modelo de request para actualizar una asignación existente
/// </summary>
public class UpdateSiteStaffRequest
{
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
}
