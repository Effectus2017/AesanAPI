namespace Api.Models.Response;

/// <summary>
/// Respuesta del calendario de visitas por agencia/sitio.
/// </summary>
public class SiteVisitCalendarResponse
{
    public int AgencyId { get; set; }

    public string AgencyName { get; set; } = string.Empty;

    public List<SiteVisitResponse> Visits { get; set; } = new();
}
