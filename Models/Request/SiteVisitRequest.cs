namespace Api.Models.Request;

/// <summary>
/// Petición para crear o actualizar una visita de sitio (calendario sponsor-evaluation).
/// </summary>
public class SiteVisitRequest
{
    public int? Id { get; set; }

    public int AgencyId { get; set; }

    public int SiteId { get; set; }

    public int VisitTypeId { get; set; }

    public DateTime VisitDate { get; set; }

    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;

    public string? Comment { get; set; }
}
