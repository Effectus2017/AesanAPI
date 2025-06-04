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

public class OperatingPolicyRepository(DapperContext context, ILogger<OperatingPolicyRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IOperatingPolicyRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<OperatingPolicyRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene una política operativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la política operativa a obtener.</param>
    /// <returns>La política operativa encontrada.</returns>
    public async Task<dynamic> GetOperatingPolicyById(int id)
    {
        string cacheKey = $"OperatingPolicy_{id}";
        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);
                var result = await db.QueryMultipleAsync("100_GetOperatingPolicyById", parameters, commandType: CommandType.StoredProcedure);
                var data = await result.ReadSingleAsync<DTOOperatingPolicy>();
                return data;
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Obtiene todas las políticas operativas.
    /// </summary>
    /// <param name="take">El número de políticas operativas a tomar.</param>
    /// <param name="skip">El número de políticas operativas a saltar.</param>
    /// <param name="name">El nombre de la política operativa a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las políticas operativas.</param>
    /// <returns>Una lista de políticas operativas.</returns>
    public async Task<dynamic> GetAllOperatingPolicies(int take, int skip, string name, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllOperatingPolicies", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOOperatingPolicy>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las políticas operativas");
            throw;
        }
    }

    /// <summary>
    /// Inserta una nueva política operativa.
    /// </summary>
    /// <param name="operatingPolicy">La política operativa a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertOperatingPolicy(DTOOperatingPolicy operatingPolicy)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", operatingPolicy.Name, DbType.String);
            parameters.Add("@nameEN", operatingPolicy.NameEN, DbType.String);
            parameters.Add("@isActive", operatingPolicy.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", operatingPolicy.DisplayOrder, DbType.Int32);
            parameters.Add("@id", operatingPolicy.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertOperatingPolicy", parameters, commandType: CommandType.StoredProcedure);
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
            _logger.LogError(ex, "Error al insertar la política operativa");
            throw;
        }
    }

    /// <summary>
    /// Actualiza una política operativa existente.
    /// </summary>
    /// <param name="operatingPolicy">La política operativa a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateOperatingPolicy(DTOOperatingPolicy operatingPolicy)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", operatingPolicy.Id, DbType.Int32);
            parameters.Add("@name", operatingPolicy.Name, DbType.String);
            parameters.Add("@nameEN", operatingPolicy.NameEN, DbType.String);
            parameters.Add("@isActive", operatingPolicy.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", operatingPolicy.DisplayOrder, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateOperatingPolicy", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(operatingPolicy.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la política operativa");
            throw;
        }
    }

    /// <summary>
    /// Elimina una política operativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la política operativa a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteOperatingPolicy(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteOperatingPolicy", parameters, commandType: CommandType.StoredProcedure);
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
            _logger.LogError(ex, "Error al eliminar la política operativa");
            throw;
        }
    }

    private void InvalidateCache(int? operatingPolicyId = null)
    {
        if (operatingPolicyId.HasValue)
        {
            _cache.Remove($"OperatingPolicy_{operatingPolicyId}");
        }
        _cache.Remove("OperatingPolicies");
        _logger.LogInformation("Cache invalidado para OperatingPolicy Repository");
    }
}