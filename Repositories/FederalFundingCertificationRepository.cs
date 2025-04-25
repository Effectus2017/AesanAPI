using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class FederalFundingCertificationRepository(DapperContext context, ILogger<FederalFundingCertificationRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IFederalFundingCertificationRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<FederalFundingCertificationRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene una certificación de fondos federales por su ID.
    /// </summary>
    /// <param name="id">El ID de la certificación a obtener.</param>
    /// <returns>La certificación encontrada.</returns>
    public async Task<dynamic> GetFederalFundingCertificationById(int id)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.FederalFundingCertification, id);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync(
                    "100_GetFederalFundingCertificationById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadSingleAsync<DTOFederalFundingCertification>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todas las certificaciones de fondos federales.
    /// </summary>
    /// <param name="take">El número de certificaciones a tomar.</param>
    /// <param name="skip">El número de certificaciones a saltar.</param>
    /// <param name="description">La descripción de la certificación a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las certificaciones.</param>
    /// <returns>Una lista de certificaciones.</returns>
    public async Task<dynamic> GetAllFederalFundingCertifications(int take, int skip, string description, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.FederalFundingCertifications, take, skip, description, alls);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32);
                parameters.Add("@skip", skip, DbType.Int32);
                parameters.Add("@description", description, DbType.String);
                parameters.Add("@alls", alls, DbType.Boolean);
                var result = await db.QueryMultipleAsync(
                    "100_GetAllFederalFundingCertifications",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadAsync<DTOFederalFundingCertification>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Inserta una nueva certificación de fondos federales.
    /// </summary>
    /// <param name="certification">La certificación a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertFederalFundingCertification(DTOFederalFundingCertification certification)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@description", certification.Description, DbType.String);
            parameters.Add("@fundingAmount", certification.FundingAmount, DbType.Decimal);
            parameters.Add("@id", certification.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync(
                "100_InsertFederalFundingCertification",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var id = parameters.Get<int>("@id");

            if (id > 0)
            {
                // Invalidar caché
                InvalidateCache(id);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la certificación de fondos federales");
            throw;
        }
    }

    /// <summary>
    /// Actualiza una certificación de fondos federales existente.
    /// </summary>
    /// <param name="certification">La certificación a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateFederalFundingCertification(DTOFederalFundingCertification certification)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", certification.Id, DbType.Int32);
            parameters.Add("@description", certification.Description, DbType.String);
            parameters.Add("@fundingAmount", certification.FundingAmount, DbType.Decimal);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UpdateFederalFundingCertification",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(certification.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la certificación de fondos federales");
            throw;
        }
    }

    /// <summary>
    /// Elimina una certificación de fondos federales por su ID.
    /// </summary>
    /// <param name="id">El ID de la certificación a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteFederalFundingCertification(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_DeleteFederalFundingCertification",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la certificación de fondos federales");
            throw;
        }
    }

    private void InvalidateCache(int? certificationId = null)
    {
        if (certificationId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.FederalFundingCertification, certificationId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.FederalFundingCertifications);
        _logger.LogInformation("Cache invalidado para FederalFundingCertification Repository");
    }
}