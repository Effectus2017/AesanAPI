using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Repositories;

public class MealTypeRepository(
    DapperContext context,
    ILogger<MealTypeRepository> logger,
    IMemoryCache cache,
    ApplicationSettings appSettings) : IMealTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<MealTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings;

    /// <summary>
    /// Obtiene un tipo de comida por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de comida a obtener.</param>
    /// <returns>El tipo de comida encontrado.</returns>
    public async Task<dynamic> GetMealTypeById(int id)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.MealType, id);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync(
                    "100_GetMealTypeById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadSingleAsync<DTOMealType>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todos los tipos de comida.
    /// </summary>
    /// <param name="take">El número de tipos de comida a tomar.</param>
    /// <param name="skip">El número de tipos de comida a saltar.</param>
    /// <param name="name">El nombre del tipo de comida a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos de comida.</param>
    /// <returns>Una lista de tipos de comida.</returns>
    public async Task<dynamic> GetAllMealTypes(int take, int skip, string name, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.MealTypes, take, skip, name, alls);

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
                    "100_GetAllMealTypes",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                var data = await result.ReadAsync<DTOMealType>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Inserta un nuevo tipo de comida.
    /// </summary>
    /// <param name="mealType">El tipo de comida a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertMealType(DTOMealType mealType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", mealType.Name, DbType.String);
            parameters.Add("@id", mealType.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync(
                "100_InsertMealType",
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
            _logger.LogError(ex, "Error al insertar el tipo de comida");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de comida existente.
    /// </summary>
    /// <param name="mealType">El tipo de comida a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateMealType(DTOMealType mealType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", mealType.Id, DbType.Int32);
            parameters.Add("@name", mealType.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UpdateMealType",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(mealType.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de comida");
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de comida por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de comida a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteMealType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_DeleteMealType",
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
            _logger.LogError(ex, "Error al eliminar el tipo de comida");
            throw;
        }
    }

    private void InvalidateCache(int? mealTypeId = null)
    {
        if (mealTypeId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.MealType, mealTypeId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.MealTypes);
        _logger.LogInformation("Cache invalidado para MealType Repository");
    }
}