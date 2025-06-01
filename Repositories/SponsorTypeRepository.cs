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

public class SponsorTypeRepository(DapperContext context, ILogger<SponsorTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : ISponsorTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SponsorTypeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    public async Task<dynamic> GetSponsorTypeById(int id)
    {
        string cacheKey = $"SponsorType_{id}";
        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync("100_GetSponsorTypeById", parameters, commandType: CommandType.StoredProcedure);
                var data = await result.ReadSingleAsync<DTOSponsorType>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    public async Task<dynamic> GetAllSponsorTypes(int take, int skip, string name, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllSponsorTypes", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOSponsorType>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de auspiciador");
            throw;
        }
    }

    public async Task<bool> InsertSponsorType(DTOSponsorType sponsorType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", sponsorType.Name, DbType.String);
            parameters.Add("@nameEN", sponsorType.NameEN, DbType.String);
            parameters.Add("@isActive", sponsorType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", sponsorType.DisplayOrder, DbType.Int32);
            parameters.Add("@id", sponsorType.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertSponsorType", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");

            if (id > 0)
            {
                InvalidateCache(id);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de auspiciador");
            throw;
        }
    }

    public async Task<bool> UpdateSponsorType(DTOSponsorType sponsorType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", sponsorType.Id, DbType.Int32);
            parameters.Add("@name", sponsorType.Name, DbType.String);
            parameters.Add("@nameEN", sponsorType.NameEN, DbType.String);
            parameters.Add("@isActive", sponsorType.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", sponsorType.DisplayOrder, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateSponsorType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(sponsorType.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de auspiciador");
            throw;
        }
    }

    public async Task<bool> DeleteSponsorType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteSponsorType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de auspiciador");
            throw;
        }
    }

    private void InvalidateCache(int? sponsorTypeId = null)
    {
        if (sponsorTypeId.HasValue)
        {
            _cache.Remove($"SponsorType_{sponsorTypeId}");
        }
        _cache.Remove("SponsorTypes");
        _logger.LogInformation("Cache invalidado para SponsorType Repository");
    }
}