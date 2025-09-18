using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class AreaTypeRepository(DapperContext context, ILogger<AreaTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : IAreaTypeRepository
{
    private readonly DapperContext _context = context;
    private readonly ILogger<AreaTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value;
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene un tipo de área por su ID
    /// </summary>
    /// <param name="id">El ID del tipo de área</param>
    /// <returns>El tipo de área obtenido</returns>
    public async Task<dynamic> GetAreaTypeById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryFirstOrDefaultAsync<DTOAreaType>("100_GetAreaTypeById", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting area type by id: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de área
    /// </summary>
    /// <param name="take">El número de tipos de área a tomar</param>
    /// <param name="skip">El número de tipos de área a saltar</param>
    /// <param name="name">El nombre del tipo de área</param>
    /// <param name="alls">Indica si se deben obtener todos los tipos de área</param>
    /// <param name="isList">Indica si se debe retornar una lista o un objeto</param>
    /// <returns>Los tipos de área obtenidos</returns>
    public async Task<dynamic> GetAllAreaTypes(int take, int skip, string name, bool alls, bool isList)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            parameters.Add("@isList", isList, DbType.Boolean);

            if (isList)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.AreaTypes, take, skip, name, alls);
                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        using var result = await db.QueryMultipleAsync("100_GetAllAreaTypes", parameters, commandType: CommandType.StoredProcedure);
                        if (result == null) { return []; }
                        var data = result.Read<dynamic>().Select(_mappingService.MapAreaTypeList).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                using var result = await db.QueryMultipleAsync("100_GetAllAreaTypes", parameters, commandType: CommandType.StoredProcedure);
                if (result == null) { return null; }
                var data = result.Read<dynamic>().Select(_mappingService.MapAreaType).ToList();
                var count = result.ReadFirstOrDefault<int>();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting area types with parameters: take={Take}, skip={Skip}, name={Name}, alls={Alls}, isList={IsList}", take, skip, name, alls, isList);
            throw;
        }
    }

    /// <summary>
    /// Inserta un tipo de área
    /// </summary>
    /// <param name="areaType">El tipo de área a insertar</param>
    /// <returns>Indica si la inserción fue exitosa</returns>
    public async Task<bool> InsertAreaType(AreaTypeRequest areaType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", areaType.Name, DbType.String);
            parameters.Add("@nameEN", areaType.NameEN, DbType.String);
            parameters.Add("@isActive", areaType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", areaType.DisplayOrder, DbType.Int32);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await db.ExecuteAsync("100_InsertAreaType", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting area type: {AreaType}", areaType);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de área
    /// </summary>
    /// <param name="areaType">El tipo de área a actualizar</param>
    /// <returns>Indica si la actualización fue exitosa</returns>
    public async Task<bool> UpdateAreaType(DTOAreaType areaType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", areaType.Id, DbType.Int32);
            parameters.Add("@name", areaType.Name, DbType.String);
            parameters.Add("@nameEN", areaType.NameEN, DbType.String);
            parameters.Add("@isActive", areaType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", areaType.DisplayOrder, DbType.Int32);
            var affected = await db.ExecuteAsync("100_UpdateAreaType", parameters, commandType: CommandType.StoredProcedure);
            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating area type: {AreaType}", areaType);
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de área
    /// </summary>
    /// <param name="id">El ID del tipo de área a eliminar</param>
    /// <returns>Indica si la eliminación fue exitosa</returns>
    public async Task<bool> DeleteAreaType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var affected = await db.ExecuteAsync("100_DeleteAreaType", parameters, commandType: CommandType.StoredProcedure);
            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area type with id {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene el tipo de área válido para una ciudad específica
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>El tipo de área válido para la ciudad</returns>
    public async Task<dynamic> GetAreaTypeByCity(int cityId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@cityId", cityId, DbType.Int32);

            var result = await db.QueryAsync<DTOAreaType>("100_GetAreaTypeByCity", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de área para la ciudad {CityId}", cityId);
            throw;
        }
    }

}