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
            _logger.LogInformation("Obteniendo todas las ciudades de la base de datos local");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new
            {
                take,
                skip,
                name,
                alls
            };

            var result = await dbConnection.QueryMultipleAsync("100_GetCities", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var model = result.Read<dynamic>().Select(item => new DTOCityTable
            {
                Id = item.Id,
                Name = item.Name,
            }).ToList();

            var count = result.Read<int>().Single();

            var _complete = new
            {
                data = model,
                count
            };

            return _complete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los customers de la base de datos local");
            throw new Exception(ex.Message);
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
            _logger.LogInformation("Obteniendo todas las regiones de la base de datos local");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new
            {
                take,
                skip,
                name,
                alls
            };

            var result = await dbConnection.QueryMultipleAsync("100_GetRegions", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var model = result.Read<dynamic>().Select(item => new DTORegionTable
            {
                Id = item.Id,
                Name = item.Name,
            }).ToList();

            var count = result.Read<int>().Single();

            var _complete = new
            {
                data = model,
                count
            };

            return _complete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones de la base de datos local");
            throw new Exception(ex.Message);
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
            _logger.LogInformation("Obteniendo ciudad por ID");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new
            {
                cityId
            };

            var result = await dbConnection.QueryAsync<dynamic>("100_GetCityById", param, commandType: CommandType.StoredProcedure);

            if (result == null || !result.Any())
            {
                return null;
            }

            var model = result.Select(item => new DTOCityTable
            {
                Id = item.Id,
                Name = item.Name,
            }).ToList();

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la ciudad por ID");
            throw new Exception(ex.Message);
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
            _logger.LogInformation("Obteniendo regiones por ID de ciudad");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new
            {
                cityId
            };

            var result = await dbConnection.QueryMultipleAsync("100_GetRegionsByCityId", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var model = result.Read<dynamic>().Select(item => new DTORegionTable
            {
                Id = item.Id,
                Name = item.Name,
            }).ToList();

            var count = result.Read<int>().Single();

            var _complete = new
            {
                data = model,
                count
            };

            return _complete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las regiones por ID de ciudad");
            throw new Exception(ex.Message);
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
            _logger.LogInformation("Obteniendo región por ID");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new
            {
                regionId
            };

            var result = await dbConnection.QueryAsync<dynamic>("100_GetRegionById", param, commandType: CommandType.StoredProcedure);

            if (result == null || !result.Any())
            {
                return null;
            }

            var model = result.Select(item => new DTORegionTable
            {
                Id = item.Id,
                Name = item.Name,
            }).FirstOrDefault();

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la región por ID");
            throw new Exception(ex.Message);
        }
    }
}
