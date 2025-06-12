using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class DeliveryTypeRepository(DapperContext context, ILogger<DeliveryTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IDeliveryTypeRepository
{
    private readonly DapperContext _context = context;
    private readonly ILogger<DeliveryTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value;

    /// <summary>
    /// Obtiene un tipo de entrega por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de entrega a obtener.</param>
    /// <returns>El tipo de entrega encontrado o null si no se encuentra.</returns>
    public async Task<dynamic> GetDeliveryTypeById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryFirstOrDefaultAsync<DTODeliveryType>("100_GetDeliveryTypeById", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de entrega con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de entrega.
    /// </summary>
    /// <param name="take">El número de tipos a tomar.</param>
    /// <param name="skip">El número de tipos a saltar.</param>
    /// <param name="name">El nombre del tipo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos.</param>
    /// <returns>Una lista de tipos de entrega y el total.</returns>
    public async Task<dynamic> GetAllDeliveryTypes(int take, int skip, string name, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllDeliveryTypes", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTODeliveryType>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de entrega");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo tipo de entrega.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertDeliveryType(DTODeliveryType deliveryType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", deliveryType.Name, DbType.String);
            parameters.Add("@nameEN", deliveryType.NameEN, DbType.String);
            parameters.Add("@isActive", deliveryType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", deliveryType.DisplayOrder, DbType.Int32);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await db.ExecuteAsync("100_InsertDeliveryType", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            if (id > 0) InvalidateCache(id);
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de entrega");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de entrega existente.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateDeliveryType(DTODeliveryType deliveryType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", deliveryType.Id, DbType.Int32);
            parameters.Add("@name", deliveryType.Name, DbType.String);
            parameters.Add("@nameEN", deliveryType.NameEN, DbType.String);
            parameters.Add("@isActive", deliveryType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", deliveryType.DisplayOrder, DbType.Int32);
            await db.ExecuteAsync("100_UpdateDeliveryType", parameters, commandType: CommandType.StoredProcedure);
            InvalidateCache(deliveryType.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de entrega");
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de entrega existente.
    /// </summary>
    /// <param name="id">El ID del tipo de entrega a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteDeliveryType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            await db.ExecuteAsync("100_DeleteDeliveryType", parameters, commandType: CommandType.StoredProcedure);
            InvalidateCache(id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de entrega");
            throw;
        }
    }

    /// <summary>
    /// Invalida el cache para el tipo de entrega.
    /// </summary>
    /// <param name="deliveryTypeId">El ID del tipo de entrega a invalidar.</param>
    private void InvalidateCache(int deliveryTypeId)
    {
        _cache.Remove(string.Format(_appSettings.Cache.Keys.DeliveryType, deliveryTypeId, "*", "*"));
        _logger.LogInformation("Cache invalidado para DeliveryType Repository");
    }
}