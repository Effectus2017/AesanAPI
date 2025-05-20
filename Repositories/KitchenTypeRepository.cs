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

    public async Task<dynamic> GetKitchenTypeById(int id)
    {
        string cacheKey = $"KitchenType_{id}";
        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync("100_GetKitchenTypeById", parameters, commandType: CommandType.StoredProcedure);
                var data = await result.ReadSingleAsync<DTOKitchenType>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    public async Task<dynamic> GetAllKitchenTypes(int take, int skip, string name, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllKitchenTypes", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOKitchenType>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de cocina");
            throw;
        }
    }

    public async Task<bool> InsertKitchenType(DTOKitchenType kitchenType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", kitchenType.Name, DbType.String);
            parameters.Add("@nameEN", kitchenType.NameEN, DbType.String);
            parameters.Add("@isActive", kitchenType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", kitchenType.DisplayOrder, DbType.Int32);
            parameters.Add("@id", kitchenType.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertKitchenType", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");

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
}