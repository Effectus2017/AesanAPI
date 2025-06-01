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

public class GroupTypeRepository(DapperContext context, ILogger<GroupTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IGroupTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<GroupTypeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    public async Task<dynamic> GetGroupTypeById(int id)
    {
        string cacheKey = $"GroupType_{id}";
        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync("100_GetGroupTypeById", parameters, commandType: CommandType.StoredProcedure);
                var data = await result.ReadSingleAsync<DTOGroupType>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    public async Task<dynamic> GetAllGroupTypes(int take, int skip, string name, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllGroupTypes", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOGroupType>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de grupo");
            throw;
        }
    }

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
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateGroupType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

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

    private void InvalidateCache(int? groupTypeId = null)
    {
        if (groupTypeId.HasValue)
        {
            _cache.Remove($"GroupType_{groupTypeId}");
        }
        _cache.Remove("GroupTypes");
        _logger.LogInformation("Cache invalidado para GroupType Repository");
    }
}