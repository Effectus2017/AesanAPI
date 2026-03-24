using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Calendario de visitas por sitio (sponsor-evaluation).
/// </summary>
public interface ISiteVisitCalendarRepository
{
    Task<SiteVisitCalendarResponse> GetVisits(int agencyId, int siteId, int? month = null, int? year = null);

    Task<IReadOnlyList<VisitTypeDropdownItemResponse>> GetVisitTypes(bool alls = false);

    Task<int?> CreateVisit(SiteVisitRequest request, string? userId);

    Task<bool> UpdateVisit(int id, SiteVisitRequest request, string? userId);

    Task<bool> DeleteVisit(int id, int agencyId, string? userId);
}
