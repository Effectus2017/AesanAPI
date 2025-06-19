using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class KitchenTypeRepository(DapperContext context, ILogger<KitchenTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IKitchenTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<KitchenTypeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un tipo de cocina por su ID
    /// </summary>
    /// <param name="id">El ID del tipo de cocina a obtener</param>
    /// <returns>El tipo de cocina encontrado</returns>
    public async Task<dynamic> GetKitchenTypeById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetKitchenTypeById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOKitchenType>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de cocina con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de cocina
    /// </summary>
    /// <param name="take">El número de tipos de cocina a obtener</param>
    /// <param name="skip">El número de tipos de cocina a saltar</param>
    /// <param name="name">El nombre de los tipos de cocina a buscar</param>
    /// <param name="alls">Si se deben obtener todos los tipos de cocina</param>
    /// <returns>Los tipos de cocina encontrados</returns>
    public async Task<dynamic> GetAllKitchenTypes(int take, int skip, string name, bool alls, bool isList)
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
                string cacheKey = string.Format(_appSettings.Cache.Keys.KitchenTypes, take, skip, name, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        using var result = await db.QueryMultipleAsync("100_GetAllKitchenTypes", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(MapKitchenTypeListFromResult).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(30)
                );
            }
            else
            {
                using var result = await db.QueryMultipleAsync("100_GetAllKitchenTypes", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(MapKitchenTypeFromResult).ToList();
                var count = result.ReadFirstOrDefault<int>();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de cocina con parámetros: take={Take}, skip={Skip}, name={Name}, alls={Alls}, isList={IsList}",
                take, skip, name, alls, isList);
            throw;
        }
    }

    /// <summary>
    /// Inserta un tipo de cocina
    /// </summary>
    /// <param name="kitchenType">El tipo de cocina a insertar</param>
    /// <returns>True si la inserción fue exitosa, false en caso contrario</returns>
    public async Task<bool> InsertKitchenType(KitchenTypeRequest kitchenType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", kitchenType.Name, DbType.String);
            parameters.Add("@nameEN", kitchenType.NameEN, DbType.String);
            parameters.Add("@isActive", kitchenType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", kitchenType.DisplayOrder, DbType.Int32);

            await db.ExecuteAsync("100_InsertKitchenType", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@Id");

            if (id > 0)
            {
                InvalidateCache(id);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de cocina");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de cocina
    /// </summary>
    /// <param name="kitchenType">El tipo de cocina a actualizar</param>
    /// <returns>True si la actualización fue exitosa, false en caso contrario</returns>
    public async Task<bool> UpdateKitchenType(DTOKitchenType kitchenType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", kitchenType.Id, DbType.Int32);
            parameters.Add("@name", kitchenType.Name, DbType.String);
            parameters.Add("@nameEN", kitchenType.NameEN, DbType.String);
            parameters.Add("@isActive", kitchenType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", kitchenType.DisplayOrder, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateKitchenType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(kitchenType.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de cocina");
            throw;
        }
    }

    /// <summary>
    /// Actualiza el orden de visualización de un tipo de cocina
    /// </summary>
    /// <param name="kitchenTypeId">El ID del tipo de cocina a actualizar</param>
    /// <param name="displayOrder">El nuevo orden de visualización</param>
    /// <returns>True si la actualización fue exitosa, false en caso contrario</returns>
    public async Task<bool> UpdateKitchenTypeDisplayOrder(int kitchenTypeId, int displayOrder)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@kitchenTypeId", kitchenTypeId, DbType.Int32);
            parameters.Add("@displayOrder", displayOrder, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("105_UpdateKitchenTypeDisplayOrder", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(kitchenTypeId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el orden de visualización del tipo de cocina");
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de cocina
    /// </summary>
    /// <param name="id">El ID del tipo de cocina a eliminar</param>
    /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
    public async Task<bool> DeleteKitchenType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteKitchenType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de cocina");
            throw;
        }
    }

    private void InvalidateCache(int? kitchenTypeId = null)
    {
        if (kitchenTypeId.HasValue)
        {
            _cache.Remove($"KitchenType_{kitchenTypeId}");
        }
        _cache.Remove("KitchenTypes");
        _logger.LogInformation("Cache invalidado para KitchenType Repository");
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una lista de tipos de cocina
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Lista de tipos de cocina</returns> 
    private static DTOKitchenType MapKitchenTypeListFromResult(dynamic result)
    {
        return new DTOKitchenType
        {
            Id = result.Id,
            Name = result.Name,
            NameEN = result.NameEN,
        };
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un tipo de cocina
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Tipo de cocina</returns>
    private static DTOKitchenType MapKitchenTypeFromResult(dynamic result)
    {
        return new DTOKitchenType
        {
            Id = result.Id,
            Name = result.Name,
            NameEN = result.NameEN,
            IsActive = result.IsActive,
            DisplayOrder = result.DisplayOrder,
        };
    }
}