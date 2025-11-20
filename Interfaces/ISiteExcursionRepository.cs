using Api.Models.Request;
using Api.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Interfaces;

public interface ISiteExcursionRepository
{
    /// <summary>
    /// Obtiene una excursión por su ID
    /// </summary>
    Task<SiteExcursionResponse?> GetSiteExcursionById(int id);

    /// <summary>
    /// Obtiene todas las excursiones de un sitio
    /// </summary>
    Task<List<SiteExcursionResponse>> GetSiteExcursionsBySiteId(int siteId, bool includeInactive = false);

    /// <summary>
    /// Obtiene excursiones de un sitio por rango de fechas
    /// </summary>
    Task<List<SiteExcursionResponse>> GetSiteExcursionsByDateRange(int siteId, DateTime startDate, DateTime endDate, bool includeInactive = false);

    /// <summary>
    /// Obtiene excursiones de un sitio por grupo de niños
    /// </summary>
    Task<List<SiteExcursionResponse>> GetSiteExcursionsByChildGroupId(int siteId, int childGroupId, bool includeInactive = false);

    /// <summary>
    /// Inserta una nueva excursión
    /// </summary>
    Task<int> InsertSiteExcursion(SiteExcursionRequest request);

    /// <summary>
    /// Actualiza una excursión existente
    /// </summary>
    Task<bool> UpdateSiteExcursion(SiteExcursionRequest request);

    /// <summary>
    /// Elimina una excursión (soft delete)
    /// </summary>
    Task<bool> DeleteSiteExcursion(int id);
}

