using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Repositories;

public class AgencyUserAssignmentRepository(
    DapperContext context,
    ILogger<AgencyUserAssignmentRepository> logger,
    IMemoryCache cache,
    ApplicationSettings appSettings) : IAgencyUserAssignmentRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyUserAssignmentRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings;

    /// <summary>
    /// Obtiene las agencias asignadas a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <returns>Lista de agencias asignadas al usuario</returns>
    public async Task<dynamic> GetUserAssignedAgencies(string userId, int take, int skip)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.AgencyUserAssignments, userId, take, skip);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.String);
                parameters.Add("@take", take, DbType.Int32);
                parameters.Add("@skip", skip, DbType.Int32);

                var result = await db.QueryMultipleAsync(
                    "100_GetUserAssignedAgencies",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var data = await result.ReadAsync<dynamic>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Asigna una agencia a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>True si la asignación fue exitosa</returns>
    public async Task<bool> AssignAgencyToUser(string userId, int agencyId, string assignedBy)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@assignedBy", assignedBy, DbType.String);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_AssignAgencyToUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var id = parameters.Get<int>("@Id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar la agencia al usuario");
            throw;
        }
    }

    /// <summary>
    /// Desasigna una agencia de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>True si la desasignación fue exitosa</returns>
    public async Task<bool> UnassignAgencyFromUser(string userId, int agencyId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UnassignAgencyFromUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(userId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desasignar la agencia del usuario");
            throw;
        }
    }

    private void InvalidateCache(string userId)
    {
        // Invalidar listas completas
        _cache.Remove(string.Format(_appSettings.Cache.Keys.AgencyUserAssignments, userId, "*", "*"));
        _logger.LogInformation("Cache invalidado para AgencyUserAssignment Repository");
    }
}