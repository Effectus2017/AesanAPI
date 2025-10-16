using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class GroupTypeRepository(DapperContext context, ILogger<GroupTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : IGroupTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<GroupTypeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene un tipo de grupo por su ID
    /// </summary>
    /// <param name="id">ID del tipo de grupo</param>
    /// <returns>Tipo de grupo</returns>
    public async Task<dynamic> GetGroupTypeById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetGroupTypeById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOGroupType>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de grupo con ID {GroupTypeId}", id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de grupo
    /// </summary>
    /// <param name="take">Número de tipos de grupo a obtener</param>
    /// <param name="skip">Número de tipos de grupo a saltar</param>
    /// <param name="name">Nombre del tipo de grupo a buscar</param>
    /// <param name="alls">Si se deben obtener todos los tipos de grupo</param>
    /// <param name="isList">Si se debe retornar una lista o un objeto con el conteo</param>
    /// <returns>Lista de tipos de grupo o un objeto con el conteo</returns>
    public async Task<dynamic> GetAllGroupTypes(int take, int skip, string name, bool alls, bool isList)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            if (isList)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.GroupTypes, take, skip, name, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await db.QueryMultipleAsync("100_GetAllGroupTypes", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(_mappingService.MapGroupTypeList).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                var result = await db.QueryMultipleAsync("100_GetAllGroupTypes", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapGroupType).ToList();
                var count = result.Read<int>().Single();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de grupo");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta un tipo de grupo
    /// </summary>
    /// <param name="groupType">Tipo de grupo a insertar</param>
    /// <returns>True si se insertó correctamente, false en caso contrario</returns>
    public async Task<bool> InsertGroupType(DTOGroupType groupType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", groupType.Name, DbType.String);
            parameters.Add("@nameEN", groupType.NameEN, DbType.String);
            parameters.Add("@isActive", groupType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", groupType.DisplayOrder, DbType.Int32);
            parameters.Add("@id", groupType.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertGroupType", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");

            if (id > 0)
            {
                InvalidateCache(id);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de grupo");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de grupo
    /// </summary>
    /// <param name="groupType">Tipo de grupo a actualizar</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    public async Task<bool> UpdateGroupType(DTOGroupType groupType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", groupType.Id, DbType.Int32);
            parameters.Add("@name", groupType.Name, DbType.String);
            parameters.Add("@nameEN", groupType.NameEN, DbType.String);
            parameters.Add("@isActive", groupType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", groupType.DisplayOrder, DbType.Int32);
            var rowsAffected = await db.ExecuteAsync("100_UpdateGroupType", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache(groupType.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de grupo");
            throw;
        }
    }

    /// <summary>
    /// Actualiza el orden de visualización de un tipo de grupo
    /// </summary>
    /// <param name="groupTypeId">ID del tipo de grupo</param>
    /// <param name="displayOrder">Nuevo orden de visualización</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    public async Task<bool> UpdateGroupTypeDisplayOrder(int groupTypeId, int displayOrder)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@groupTypeId", groupTypeId, DbType.Int32);
            parameters.Add("@displayOrder", displayOrder, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("105_UpdateGroupTypeDisplayOrder", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(groupTypeId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el orden de visualización del tipo de grupo");
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de grupo
    /// </summary>
    /// <param name="id">ID del tipo de grupo</param>
    /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
    public async Task<bool> DeleteGroupType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteGroupType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de grupo");
            throw;
        }
    }

    /// <summary>
    /// Obtiene la ubicación del sitio por tipo de grupo
    /// </summary>
    /// <param name="groupTypeId">ID del tipo de grupo</param>
    /// <returns>Ubicación del sitio</returns>
    public async Task<dynamic> GetSiteLocationByGroupType(int groupTypeId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@groupTypeId", groupTypeId, DbType.Int32);

            var result = await db.QueryAsync<DTOOptionSelection>("100_GetSiteLocationByGroupType", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la ubicación del sitio para el tipo de grupo con ID {GroupTypeId}", groupTypeId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Invalida el caché para el tipo de grupo
    /// </summary>
    /// <param name="groupTypeId">ID del tipo de grupo</param>
    private void InvalidateCache(int? groupTypeId = null)
    {
        if (groupTypeId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.GroupTypes, 0, 0, "", false));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.GroupTypes);
        _logger.LogInformation("Cache invalidado para GroupType Repository");
    }

}