using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class AreaTypeRepository(DapperContext context, ILogger<AreaTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IAreaTypeRepository
{
    private readonly DapperContext _context = context;
    private readonly ILogger<AreaTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value;

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
                        var data = result.Read<dynamic>().Select(MapAreaTypeListFromResult).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(30)
                );
            }
            else
            {
                using var result = await db.QueryMultipleAsync("100_GetAllAreaTypes", parameters, commandType: CommandType.StoredProcedure);
                if (result == null) { return null; }
                var data = result.Read<dynamic>().Select(MapAreaTypeFromResult).ToList();
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

    private static DTOAreaType MapAreaTypeListFromResult(dynamic result)
    {
        return new DTOAreaType
        {
            Id = result.Id,
            Name = result.Name,
            NameEN = result.NameEN
        };
    }

    private static DTOAreaType MapAreaTypeFromResult(dynamic result)
    {
        return new DTOAreaType
        {
            Id = result.Id,
            Name = result.Name,
            NameEN = result.NameEN,
            IsActive = result.IsActive,
            DisplayOrder = result.DisplayOrder
        };
    }
}