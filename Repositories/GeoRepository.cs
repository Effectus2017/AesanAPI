using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class GeoRepository(ILogger<GeoRepository> logger, DapperContext context, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IGeoRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<GeoRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene todas las ciudades de la base de datos local
    /// </summary>
    /// <param name="take">El número de registros a tomar</param>
    /// <param name="skip">El número de registros a saltar</param>
    /// <param name="name">El nombre de la ciudad</param>
    /// <param name="alls">Si se deben obtener todas las ciudades</param>
    /// <returns>Las ciudades</returns>
    public async Task<dynamic> GetAllCitiesFromDb(int take, int skip, string name, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Cities, take, skip, name, alls);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32);
                parameters.Add("@skip", skip, DbType.Int32);
                parameters.Add("@name", name, DbType.String);
                parameters.Add("@alls", alls, DbType.Boolean);

                var result = await db.QueryMultipleAsync(
                    "100_GetCities",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = result
                    .Read<dynamic>()
                    .Select(item => new DTOCity { Id = item.Id, Name = item.Name })
                    .ToList();

                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todas las regiones de la base de datos local
    /// </summary>
    /// <param name="take">El número de registros a tomar</param>
    /// <param name="skip">El número de registros a saltar</param>
    /// <param name="name">El nombre de la región</param>
    /// <param name="alls">Si se deben obtener todas las regiones</param>
    /// <returns>Las regiones</returns>
    public async Task<dynamic> GetAllRegionsFromDb(int take, int skip, string name, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Regions, take, skip, name, alls);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32);
                parameters.Add("@skip", skip, DbType.Int32);
                parameters.Add("@name", name, DbType.String);
                parameters.Add("@alls", alls, DbType.Boolean);

                var result = await db.QueryMultipleAsync(
                    "100_GetRegions",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = result
                    .Read<dynamic>()
                    .Select(item => new DTORegion { Id = item.Id, Name = item.Name })
                    .ToList();

                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene una ciudad por su ID
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>La ciudad</returns>
    public async Task<dynamic> GetCityById(int cityId)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.City, cityId);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", cityId, DbType.Int32);
                return await db.QueryFirstOrDefaultAsync<dynamic>(
                    "100_GetCityById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene las regiones por ID de ciudad
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>Las regiones</returns>
    public async Task<dynamic> GetRegionsByCityId(int cityId)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.RegionsByCity, cityId);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@cityId", cityId, DbType.Int32);

                var result = await db.QueryMultipleAsync(
                    "100_GetRegionsByCityId",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = result
                    .Read<dynamic>()
                    .Select(item => new DTORegion { Id = item.Id, Name = item.Name })
                    .ToList();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene una región por su ID
    /// </summary>
    /// <param name="regionId">El ID de la región</param>
    /// <returns>La región</returns>
    public async Task<dynamic> GetRegionById(int regionId)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Region, regionId);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", regionId, DbType.Int32);
                return await db.QueryFirstOrDefaultAsync<dynamic>(
                    "100_GetRegionById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene las ciudades disponibles para una región específica
    /// </summary>
    /// <param name="regionId">El ID de la región</param>
    /// <returns>Las ciudades asociadas a la región</returns>
    public async Task<dynamic> GetCitiesByRegionId(int regionId)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.CitiesByRegion, regionId);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@regionId", regionId, DbType.Int32);

                var result = await db.QueryMultipleAsync(
                    "100_GetCitiesByRegionId",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadAsync<dynamic>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    // Método para invalidar el caché manualmente si es necesario
    public void InvalidateCache(int? cityId = null, int? regionId = null)
    {
        if (cityId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.City, cityId));
            _cache.Remove(string.Format(_appSettings.Cache.Keys.RegionsByCity, cityId));
        }

        if (regionId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Region, regionId));
            _cache.Remove(string.Format(_appSettings.Cache.Keys.CitiesByRegion, regionId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Cities);
        _cache.Remove(_appSettings.Cache.Keys.Regions);

        _logger.LogInformation("Cache invalidado para Geo Repository");
    }
}
