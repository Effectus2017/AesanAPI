using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class GeoRepository(ILogger<GeoRepository> logger, DapperContext context) : IGeoRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<GeoRepository> _logger = logger;

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
        try
        {
            _logger.LogInformation("Obteniendo todas las ciudades");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            var result = await db.QueryMultipleAsync("100_GetCities", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<dynamic>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ciudades");
            throw;
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
    public async Task<dynamic> GetAllRegionsFromDb(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las regiones");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            var result = await db.QueryMultipleAsync("100_GetRegions", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<dynamic>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones");
            throw;
        }
    }

    /// <summary>
    /// Obtiene una ciudad por su ID
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>La ciudad</returns>
    public async Task<dynamic> GetCityById(int cityId)
    {
        try
        {
            _logger.LogInformation("Obteniendo ciudad por ID: {CityId}", cityId);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", cityId, DbType.Int32);

            return await db.QueryFirstOrDefaultAsync<dynamic>("100_GetCityById", parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la ciudad por ID: {CityId}", cityId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene las regiones por ID de ciudad
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>Las regiones</returns>
    public async Task<dynamic> GetRegionsByCityId(int cityId)
    {
        try
        {
            _logger.LogInformation("Obteniendo regiones por ID de ciudad: {CityId}", cityId);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@cityId", cityId, DbType.Int32);

            var result = await db.QueryMultipleAsync("100_GetRegionsByCityId", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<dynamic>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones por ID de ciudad: {CityId}", cityId);
            throw;
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
            _logger.LogInformation("Obteniendo región por ID: {RegionId}", regionId);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", regionId, DbType.Int32);

            return await db.QueryFirstOrDefaultAsync<dynamic>("100_GetRegionById", parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la región por ID: {RegionId}", regionId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene las ciudades disponibles para una región específica
    /// </summary>
    /// <param name="regionId">El ID de la región</param>
    /// <returns>Las ciudades asociadas a la región</returns>
    public async Task<dynamic> GetCitiesByRegionId(int regionId)
    {
        try
        {
            _logger.LogInformation("Obteniendo ciudades por ID de región: {RegionId}", regionId);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@regionId", regionId, DbType.Int32);

            var result = await db.QueryMultipleAsync("100_GetCitiesByRegionId", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<dynamic>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las ciudades por ID de región: {RegionId}", regionId);
            throw;
        }
    }
}
