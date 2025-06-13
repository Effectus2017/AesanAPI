using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class EducationLevelRepository(DapperContext context, ILogger<EducationLevelRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IEducationLevelRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<EducationLevelRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un nivel educativo por su ID.
    /// </summary>
    /// <param name="id">El ID del nivel educativo a obtener.</param>
    /// <returns>El nivel educativo encontrado.</returns>
    public async Task<dynamic> GetEducationLevelById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync(
                "100_GetEducationLevelById",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var data = await result.ReadSingleAsync<DTOEducationLevel>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el nivel educativo con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los niveles educativos.
    /// </summary>
    /// <param name="take">El número de niveles educativos a tomar.</param>
    /// <param name="skip">El número de niveles educativos a saltar.</param>
    /// <param name="name">El nombre del nivel educativo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los niveles educativos.</param>
    /// <returns>Una lista de niveles educativos.</returns>
    public async Task<dynamic> GetAllEducationLevels(int take, int skip, string name, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllEducationLevels", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOEducationLevel>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los niveles educativos");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo nivel educativo.
    /// </summary>
    /// <param name="educationLevel">El nivel educativo a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertEducationLevel(DTOEducationLevel educationLevel)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", educationLevel.Name, DbType.String);
            parameters.Add("@id", educationLevel.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync(
                "100_InsertEducationLevel",
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
            _logger.LogError(ex, "Error al insertar el nivel educativo");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un nivel educativo existente.
    /// </summary>
    /// <param name="educationLevel">El nivel educativo a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateEducationLevel(DTOEducationLevel educationLevel)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", educationLevel.Id, DbType.Int32);
            parameters.Add("@name", educationLevel.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UpdateEducationLevel",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(educationLevel.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el nivel educativo");
            throw;
        }
    }

    /// <summary>
    /// Elimina un nivel educativo por su ID.
    /// </summary>
    /// <param name="id">El ID del nivel educativo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteEducationLevel(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_DeleteEducationLevel",
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
            _logger.LogError(ex, "Error al eliminar el nivel educativo");
            throw;
        }
    }

    private void InvalidateCache(int? educationLevelId = null)
    {
        if (educationLevelId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.EducationLevel, educationLevelId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.EducationLevels);
        _logger.LogInformation("Cache invalidado para EducationLevel Repository");
    }
}