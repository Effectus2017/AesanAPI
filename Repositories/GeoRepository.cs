using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Errors;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class GeoRepository(DapperContext context, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : IGeoRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene una ciudad por su ID
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>La ciudad</returns>
    public async Task<dynamic> GetCityById(int cityId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", cityId, DbType.Int32);
            var result = await db.QueryFirstOrDefaultAsync<DTOCity>("100_GetCityById", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return Array.Empty<DTOCity>();
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener la ciudad por ID: {cityId}", ex);
        }
    }

    /// <summary>
    /// Obtiene una región por su ID
    /// </summary>
    /// <param name="regionId">El ID de la región</param>
    /// <returns>La región</returns>
    public async Task<dynamic> GetRegionById(int regionId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", regionId, DbType.Int32);
            var result = await db.QueryFirstOrDefaultAsync<DTORegion>("100_GetRegionById", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return Array.Empty<DTORegion>();
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener la región por ID: {regionId}", ex);
        }
    }


    /// <summary>
    /// Obtiene todas las ciudades de la base de datos local
    /// </summary>
    /// <param name="take">El número de registros a tomar</param>
    /// <param name="skip">El número de registros a saltar</param>
    /// <param name="name">El nombre de la ciudad</param>
    /// <param name="alls">Si se deben obtener todas las ciudades</param>
    /// <returns>Las ciudades</returns>
    public async Task<dynamic> GetAllCitiesFromDb(int take, int skip, string name, bool alls, bool forDropdown)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@take", take, DbType.Int32);
        parameters.Add("@skip", skip, DbType.Int32);
        parameters.Add("@name", name, DbType.String);
        parameters.Add("@alls", alls, DbType.Boolean);

        if (forDropdown)
        {
            string cacheKey = string.Format(_appSettings.Cache.Keys.Cities, take, skip, name, alls);

            return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                var result = await db.QueryMultipleAsync("100_GetCities", parameters, commandType: CommandType.StoredProcedure);
                var data = result.Read<DTOCity>().Select(_mappingService.MapCityList).ToList();
                return data;
            },
            _appSettings,
            TimeSpan.FromMinutes(1)
        );
        }
        else
        {
            using var result = await db.QueryMultipleAsync("100_GetCities", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new { data = Array.Empty<DTOCity>(), count = 0 };
            }

            var data = result.Read<DTOCity>().Select(_mappingService.MapCityList).ToList();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }

    }

    /// <summary>
    /// Obtiene todas las regiones de la base de datos local
    /// </summary>
    /// <param name="take">El número de registros a tomar</param>
    /// <param name="skip">El número de registros a saltar</param>
    /// <param name="name">El nombre de la región</param>
    /// <param name="alls">Si se deben obtener todas las regiones</param>
    /// <returns>Las regiones</returns>
    public async Task<dynamic> GetAllRegionsFromDb(int take, int skip, string name, bool alls, bool forDropdown)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@take", take, DbType.Int32);
        parameters.Add("@skip", skip, DbType.Int32);
        parameters.Add("@name", name, DbType.String);
        parameters.Add("@alls", alls, DbType.Boolean);

        if (forDropdown)
        {
            string cacheKey = string.Format(_appSettings.Cache.Keys.Regions, take, skip, name, alls);

            return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                var result = await db.QueryMultipleAsync("100_GetRegions", parameters, commandType: CommandType.StoredProcedure);
                var data = result.Read<DTORegion>().Select(_mappingService.MapRegionList).ToList();
                return data;
            },
            _appSettings,
            TimeSpan.FromMinutes(1)
        );
        }
        else
        {
            using var result = await db.QueryMultipleAsync("100_GetRegions", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new { data = Array.Empty<DTORegion>(), count = 0 };
            }

            var data = result.Read<DTORegion>().Select(_mappingService.MapRegionList).ToList();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
    }

    /// <summary>
    /// Obtiene las regiones por ID de ciudad
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>Las regiones</returns>
    public async Task<dynamic> GetRegionsByCityId(int cityId, bool forDropdown)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@cityId", cityId, DbType.Int32);

            if (forDropdown)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.RegionsByCity, cityId);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await db.QueryMultipleAsync("100_GetRegionsByCityId", parameters, commandType: CommandType.StoredProcedure);
                        var data = result.Read<DTORegion>().Select(_mappingService.MapRegionList).ToList();
                        return data;
                    },
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                using var result = await db.QueryMultipleAsync("100_GetRegionsByCityId", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return new { data = Array.Empty<DTORegion>(), count = 0 };
                }

                var data = result.Read<DTORegion>().Select(_mappingService.MapRegionList).ToList();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener las regiones por ID de ciudad: {cityId}", ex);
        }
    }


    /// <summary>
    /// Obtiene las ciudades disponibles para una región específica
    /// </summary>
    /// <param name="regionId">El ID de la región</param>
    /// <returns>Las ciudades asociadas a la región</returns>
    public async Task<dynamic> GetCitiesByRegionId(int regionId, bool forDropdown)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@regionId", regionId, DbType.Int32);

            if (forDropdown)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.CitiesByRegion, regionId);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await db.QueryMultipleAsync("100_GetCitiesByRegionId", parameters, commandType: CommandType.StoredProcedure);
                        var data = result.Read<DTOCity>().Select(_mappingService.MapCityList).ToList();
                        return data;
                    },
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                using var result = await db.QueryMultipleAsync("100_GetCitiesByRegionId", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return new { data = Array.Empty<DTOCity>(), count = 0 };
                }

                var data = result.Read<DTOCity>().Select(_mappingService.MapCityList).ToList();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener las ciudades por ID de región: {regionId}", ex);
        }
    }

    /// <summary>
    /// Invalida el caché de la base de datos local
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <param name="regionId">El ID de la región</param>
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
    }
}
