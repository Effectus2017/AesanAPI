using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

/// <summary>
/// Repositorio para operaciones con SchoolSite
/// </summary>
public class SchoolSiteRepository(DapperContext context, ILogger<SchoolSiteRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : ISchoolSiteRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SchoolSiteRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene todos los Sites asignados a una School específica con paginación
    /// </summary>
    public async Task<dynamic> GetSchoolSitesBySchoolId(int schoolId, int take = 50, int skip = 0, string? name = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", string.IsNullOrWhiteSpace(name) ? null : name?.Trim(), DbType.String);

            using var result = await dbConnection.QueryMultipleAsync("100_GetSchoolSitesBySchoolId", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new { data = Array.Empty<SchoolSiteResponse>(), count = 0 };
            }

            var schoolSites = result.Read<dynamic>().Select(_mappingService.MapSchoolSite).ToList();
            var count = result.ReadFirstOrDefault<int>();

            return new { data = schoolSites, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sites por school {SchoolId}: {Message}", schoolId, ex.Message);
            throw new Exception($"Error al obtener sites por school: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene la School asignada a un Site específico
    /// </summary>
    public async Task<SchoolSiteResponse?> GetSchoolSiteBySiteId(int siteId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetSchoolSiteBySiteId", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            return _mappingService.MapSchoolSite(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener school por site {SiteId}: {Message}", siteId, ex.Message);
            throw new Exception($"Error al obtener school por site: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Asigna un Site a una School
    /// </summary>
    public async Task<bool> InsertSchoolSite(SchoolSiteRequest request, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        IDbConnection? dbConnection = null;
        var shouldDisposeConnection = connection == null;

        try
        {
            dbConnection = connection ?? _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", request.SchoolId, DbType.Int32);
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@comment", request.Comment, DbType.String);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rowsAffected = await dbConnection.ExecuteAsync("100_InsertSchoolSite", parameters, transaction, commandType: CommandType.StoredProcedure);

            int id = parameters.Get<int>("@id");

            if (id == 0)
            {
                _logger.LogError("Error al insertar la asignación School-Site");
                return false;
            }

            InvalidateCache();

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la asignación School-Site: {Message}", ex.Message);
            throw new Exception($"Error al insertar la asignación School-Site: {ex.Message}", ex);
        }
        finally
        {
            if (shouldDisposeConnection && dbConnection != null)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Actualiza una asignación School-Site
    /// </summary>
    public async Task<bool> UpdateSchoolSite(SchoolSiteRequest request)
    {
        try
        {
            if (!request.Id.HasValue)
            {
                throw new ArgumentException("El ID de la asignación es requerido para actualizar");
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", request.Id.Value, DbType.Int32);
            parameters.Add("@schoolId", request.SchoolId, DbType.Int32);
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@comment", request.Comment, DbType.String);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);

            var rowsAffected = await dbConnection.ExecuteAsync("100_UpdateSchoolSite", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache();
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la asignación School-Site: {Message}", ex.Message);
            throw new Exception($"Error al actualizar la asignación School-Site: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cuenta cuántos sitios con tipo de grupo Comedor tiene una escuela.
    /// </summary>
    public async Task<int> CountSitesWithComedorGroupTypeBySchoolId(int schoolId, int? excludeSiteId = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@excludeSiteId", excludeSiteId, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("102_CountSitesWithComedorGroupTypeBySchoolId", parameters, commandType: CommandType.StoredProcedure);
            return result != null ? (int)result.cnt : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al contar sitios Comedor por school {SchoolId}: {Message}", schoolId, ex.Message);
            throw new Exception($"Error al contar sitios Comedor por escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Elimina una asignación School-Site (soft delete)
    /// </summary>
    public async Task<bool> DeleteSchoolSite(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var rowsAffected = await dbConnection.ExecuteAsync("100_DeleteSchoolSite", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache();
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la asignación School-Site: {Message}", ex.Message);
            throw new Exception($"Error al eliminar la asignación School-Site: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Invalida la caché para las asignaciones School-Site
    /// </summary>
    private void InvalidateCache()
    {
        _cache.Remove(_appSettings.Cache.Keys.SchoolSites);
        _logger.LogInformation("Cache invalidado para SchoolSite Repository");
    }
}
