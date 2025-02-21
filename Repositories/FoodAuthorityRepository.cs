using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Repositories;

public class FoodAuthorityRepository(
    DapperContext context,
    ILogger<FoodAuthorityRepository> logger,
    IMemoryCache cache,
    ApplicationSettings appSettings) : IFoodAuthorityRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<FoodAuthorityRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings;

    /// <summary>
    /// Obtiene una autoridad alimentaria por su ID.
    /// </summary>
    /// <param name="id">El ID de la autoridad alimentaria a obtener.</param>
    /// <returns>La autoridad alimentaria encontrada.</returns>
    public async Task<dynamic> GetFoodAuthorityById(int id)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.FoodAuthority, id);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync(
                    "100_GetFoodAuthorityById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadSingleAsync<DTOFoodAuthority>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todas las autoridades alimentarias.
    /// </summary>
    /// <param name="take">El número de autoridades alimentarias a tomar.</param>
    /// <param name="skip">El número de autoridades alimentarias a saltar.</param>
    /// <param name="name">El nombre de la autoridad alimentaria a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las autoridades alimentarias.</param>
    /// <returns>Una lista de autoridades alimentarias.</returns>
    public async Task<dynamic> GetAllFoodAuthorities(int take, int skip, string name, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.FoodAuthorities, take, skip, name, alls);

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
                    "100_GetAllFoodAuthorities",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadAsync<DTOFoodAuthority>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Inserta una nueva autoridad alimentaria.
    /// </summary>
    /// <param name="foodAuthority">La autoridad alimentaria a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertFoodAuthority(DTOFoodAuthority foodAuthority)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", foodAuthority.Name, DbType.String);
            parameters.Add("@id", foodAuthority.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync(
                "100_InsertFoodAuthority",
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
            _logger.LogError(ex, "Error al insertar la autoridad alimentaria");
            throw;
        }
    }

    /// <summary>
    /// Actualiza una autoridad alimentaria existente.
    /// </summary>
    /// <param name="foodAuthority">La autoridad alimentaria a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateFoodAuthority(DTOFoodAuthority foodAuthority)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", foodAuthority.Id, DbType.Int32);
            parameters.Add("@name", foodAuthority.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UpdateFoodAuthority",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(foodAuthority.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la autoridad alimentaria");
            throw;
        }
    }

    /// <summary>
    /// Elimina una autoridad alimentaria por su ID.
    /// </summary>
    /// <param name="id">El ID de la autoridad alimentaria a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteFoodAuthority(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_DeleteFoodAuthority",
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
            _logger.LogError(ex, "Error al eliminar la autoridad alimentaria");
            throw;
        }
    }

    private void InvalidateCache(int? foodAuthorityId = null)
    {
        if (foodAuthorityId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.FoodAuthority, foodAuthorityId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.FoodAuthorities);
        _logger.LogInformation("Cache invalidado para FoodAuthority Repository");
    }
}