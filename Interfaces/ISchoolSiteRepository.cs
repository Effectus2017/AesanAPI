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
    Task<dynamic> GetSchoolSitesBySchoolId(int schoolId, int take = 50, int skip = 0, string? name = null);

    /// <summary>
    /// Obtiene la School asignada a un Site específico
    /// </summary>
    Task<SchoolSiteResponse?> GetSchoolSiteBySiteId(int siteId);

    /// <summary>
    /// Asigna un Site a una School
    /// </summary>
    Task<bool> InsertSchoolSite(SchoolSiteRequest request, IDbConnection? connection = null, IDbTransaction? transaction = null);

    /// <summary>
    /// Actualiza una asignación School-Site
    /// </summary>
    Task<bool> UpdateSchoolSite(SchoolSiteRequest request);

    /// <summary>
    /// Elimina una asignación School-Site (soft delete)
    /// </summary>
    Task<bool> DeleteSchoolSite(int id);

    /// <summary>
    /// Cuenta cuántos sitios con tipo de grupo Comedor tiene una escuela.
    /// Usado para validar regla: solo un sitio Comedor por escuela.
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="excludeSiteId">ID del sitio a excluir del conteo (ej. en actualización)</param>
    Task<int> CountSitesWithComedorGroupTypeBySchoolId(int schoolId, int? excludeSiteId = null);

    /// <summary>
    /// Cuenta cuántos sitios activos tiene una escuela.
    /// Usado para validar regla: el primer sitio de la escuela debe ser Comedor.
    /// </summary>
    Task<int> CountSitesBySchoolId(int schoolId);

    /// <summary>
    /// Obtiene el rango de fechas de funcionamiento (OperatingFromDate, OperatingToDate) del sitio Comedor de la escuela.
    /// Si la escuela no tiene sitio Comedor, devuelve null.
    /// </summary>
    Task<(DateTime? From, DateTime? To)?> GetComedorOperatingDateRangeBySchoolId(int schoolId);
}
