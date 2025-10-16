using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Api.Services;
using System.Data;
using Api.Data;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de SchoolSite
/// </summary>
public interface ISchoolSiteRepository
{
    /// <summary>
    /// Obtiene todos los Sites asignados a una School específica
    /// </summary>
    Task<dynamic> GetSchoolSitesBySchoolId(int schoolId, int take = 50, int skip = 0);

    /// <summary>
    /// Obtiene la School asignada a un Site específico
    /// </summary>
    Task<SchoolSiteResponse?> GetSchoolSiteBySiteId(int siteId);

    /// <summary>
    /// Asigna un Site a una School
    /// </summary>
    Task<bool> InsertSchoolSite(SchoolSiteRequest request);

    /// <summary>
    /// Actualiza una asignación School-Site
    /// </summary>
    Task<bool> UpdateSchoolSite(SchoolSiteRequest request);

    /// <summary>
    /// Elimina una asignación School-Site (soft delete)
    /// </summary>
    Task<bool> DeleteSchoolSite(int id);
}
