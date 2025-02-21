using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Repositories;

public class OrganizationTypeRepository(
    DapperContext context,
    ILogger<OrganizationTypeRepository> logger,
    IMemoryCache cache,
    ApplicationSettings appSettings) : IOrganizationTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<OrganizationTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings;

    /// <summary>
    /// Obtiene un tipo de organización por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de organización a obtener.</param>
    /// <returns>El tipo de organización encontrado.</returns>
    public async Task<dynamic> GetOrganizationTypeById(int id)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.OrganizationType, id);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync(
                    "100_GetOrganizationTypeById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadSingleAsync<DTOOrganizationType>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todos los tipos de organización.
    /// </summary>
    /// <param name="take">El número de tipos de organización a tomar.</param>
    /// <param name="skip">El número de tipos de organización a saltar.</param>
    /// <param name="name">El nombre del tipo de organización a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos de organización.</param>
    /// <returns>Una lista de tipos de organización.</returns>
    public async Task<dynamic> GetAllOrganizationTypes(int take, int skip, string name, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.OrganizationTypes, take, skip, name, alls);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32);
                parameters.Add("@skip", skip, DbType.Int32);
                parameters.Add("@name", name, DbType.String);
                parameters.Add("@alls", alls, DbType.Boolean);
                var result = await db.QueryMultipleAsync(
                    "100_GetAllOrganizationTypes",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadAsync<DTOOrganizationType>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Inserta un nuevo tipo de organización.
    /// </summary>
    /// <param name="organizationType">El tipo de organización a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertOrganizationType(DTOOrganizationType organizationType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", organizationType.Name, DbType.String);
            parameters.Add("@id", organizationType.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync(
                "100_InsertOrganizationType",
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
            _logger.LogError(ex, "Error al insertar el tipo de organización");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de organización existente.
    /// </summary>
    /// <param name="organizationType">El tipo de organización a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateOrganizationType(DTOOrganizationType organizationType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", organizationType.Id, DbType.Int32);
            parameters.Add("@name", organizationType.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UpdateOrganizationType",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(organizationType.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de organización");
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de organización por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de organización a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteOrganizationType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_DeleteOrganizationType",
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
            _logger.LogError(ex, "Error al eliminar el tipo de organización");
            throw;
        }
    }

    private void InvalidateCache(int? organizationTypeId = null)
    {
        if (organizationTypeId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.OrganizationType, organizationTypeId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.OrganizationTypes);
        _logger.LogInformation("Cache invalidado para OrganizationType Repository");
    }
}