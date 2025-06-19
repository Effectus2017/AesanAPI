using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace Api.Repositories;

public class OperatingPeriodRepository(DapperContext context, ILogger<OperatingPeriodRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IOperatingPeriodRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<OperatingPeriodRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un período operativo por su ID.
    /// </summary>
    /// <param name="id">El ID del período operativo a obtener.</param>
    /// <returns>El período operativo encontrado.</returns>
    public async Task<dynamic> GetOperatingPeriodById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetOperatingPeriodById", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var data = await result.ReadSingleAsync<DTOOperatingPeriod>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting operating period by id: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los períodos operativos.
    /// </summary>
    /// <param name="take">El número de períodos operativos a tomar.</param>
    /// <param name="skip">El número de períodos operativos a saltar.</param>
    /// <param name="name">El nombre del período operativo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los períodos operativos.</param>
    /// <returns>Una lista de períodos operativos.</returns>
    public async Task<dynamic> GetAllOperatingPeriods(int take, int skip, string name, bool alls, bool isList)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            if (isList)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.OperatingPeriods, take, skip, name, alls);
                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        using var result = await db.QueryMultipleAsync("100_GetAllOperatingPeriods", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(MapOperatingPeriodListFromResult).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(30)
                );
            }
            else
            {
                using var result = await db.QueryMultipleAsync("100_GetAllOperatingPeriods", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(MapOperatingPeriodFromResult).ToList();
                var count = result.ReadFirstOrDefault<int>();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all operating periods");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo período operativo.
    /// </summary>
    /// <param name="operatingPeriod">El período operativo a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertOperatingPeriod(DTOOperatingPeriod operatingPeriod)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", operatingPeriod.Name, DbType.String);
            parameters.Add("@id", operatingPeriod.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync(
                "100_InsertOperatingPeriod",
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
            _logger.LogError(ex, "Error al insertar el período operativo");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un período operativo existente.
    /// </summary>
    /// <param name="operatingPeriod">El período operativo a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateOperatingPeriod(DTOOperatingPeriod operatingPeriod)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", operatingPeriod.Id, DbType.Int32);
            parameters.Add("@name", operatingPeriod.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UpdateOperatingPeriod",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(operatingPeriod.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el período operativo");
            throw;
        }
    }

    /// <summary>
    /// Elimina un período operativo por su ID.
    /// </summary>
    /// <param name="id">El ID del período operativo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteOperatingPeriod(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_DeleteOperatingPeriod",
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
            _logger.LogError(ex, "Error al eliminar el período operativo");
            throw;
        }
    }

    /// <summary>
    /// Invalida el caché de un período operativo
    /// </summary>
    /// <param name="operatingPeriodId">El ID del período operativo a invalidar</param>
    private void InvalidateCache(int? operatingPeriodId = null)
    {
        if (operatingPeriodId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.OperatingPeriods, 0, 0, "", false));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.OperatingPeriods);
        _logger.LogInformation("Cache invalidado para OperatingPeriod Repository");
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una lista de períodos operativos
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Lista de períodos operativos</returns>
    private static DTOOperatingPeriod MapOperatingPeriodListFromResult(dynamic result)
    {
        return new DTOOperatingPeriod
        {
            Id = result.Id,
            Name = result.Name,
            NameEN = result.NameEN,
        };
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un período operativo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Período operativo</returns>
    private static DTOOperatingPeriod MapOperatingPeriodFromResult(dynamic result)
    {
        return new DTOOperatingPeriod
        {
            Id = result.Id,
            Name = result.Name,
            NameEN = result.NameEN,
            IsActive = result.IsActive,
            DisplayOrder = result.DisplayOrder,
        };
    }
}