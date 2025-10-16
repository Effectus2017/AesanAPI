using System.Data;
using Dapper;
using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

/// <summary>
/// Repositorio para la gestión de instalaciones de sitios
/// Implementa las operaciones CRUD para instalaciones específicas de sitios
/// </summary>
public class SiteFacilityRepository(DapperContext context, ILogger<SiteFacilityRepository> logger) : ISiteFacilityRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SiteFacilityRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene todas las instalaciones de un sitio específico
    /// </summary>
    public async Task<List<SiteFacilityResponse>> GetFacilitiesBySite(int siteId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            var result = await dbConnection.QueryAsync<SiteFacilityResponse>("100_GetFacilitiesBySite", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Se obtuvieron {Count} instalaciones para el sitio {SiteId}", result.Count(), siteId);

            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener instalaciones para el sitio {SiteId}", siteId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza las instalaciones de un sitio
    /// </summary>
    public async Task<bool> UpdateSiteFacilities(int siteId, List<int> facilityTypeIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@facilityTypeIds", string.Join(",", facilityTypeIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSiteFacilities", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Instalaciones actualizadas exitosamente para el sitio {SiteId}", siteId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar instalaciones para el sitio {SiteId}", siteId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza el estado activo de las instalaciones de un sitio
    /// </summary>
    public async Task<bool> UpdateSiteFacilityIsActive(int siteId, bool isActive)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@site_id", siteId, DbType.Int32);
            parameters.Add("@is_active", isActive, DbType.Boolean);

            await dbConnection.ExecuteAsync("102_UpdateSiteFacilityIsActive", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Estado de instalaciones actualizado para el sitio {SiteId}: {IsActive}", siteId, isActive);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de instalaciones para el sitio {SiteId}", siteId);
            throw;
        }
    }

    /// <summary>
    /// Elimina todas las instalaciones de un sitio
    /// </summary>
    public async Task<bool> DeleteSiteFacilities(int siteId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            await dbConnection.ExecuteAsync("100_DeleteSiteFacilitiesBySiteId", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Instalaciones eliminadas exitosamente para el sitio {SiteId}", siteId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar instalaciones para el sitio {SiteId}", siteId);
            throw;
        }
    }
}
