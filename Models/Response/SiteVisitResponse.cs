namespace Api.Models.Response;

/// <summary>
/// Fila de visita para el calendario (sitio + tipo de visita).
/// </summary>
public class SiteVisitResponse
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public string SiteName { get; set; } = string.Empty;

    public int VisitTypeId { get; set; }

    public string VisitTypeCode { get; set; } = string.Empty;

    public string VisitTypeNameEs { get; set; } = string.Empty;

    public string VisitTypeNameEN { get; set; } = string.Empty;

    public string Date { get; set; } = string.Empty;

    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;

    public string? Comment { get; set; }
}
