using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class CenterTypeRepository(DapperContext context, ILogger<CenterTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : ICenterTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<CenterTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un tipo de centro por su ID
    /// </summary>
    /// <param name="id">El ID del tipo de centro a obtener</param>
    /// <returns>El tipo de centro encontrado</returns>
    public async Task<dynamic> GetCenterTypeById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id);
            var result = await db.QueryMultipleAsync("100_GetCenterTypeById", param, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOCenterType>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de centro con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de centro
    /// </summary>
    /// <param name="take">El número de tipos de centro a obtener</param>
    /// <param name="skip">El número de tipos de centro a saltar</param>
    /// <param name="name">Los nombres de los tipos de centro a buscar (separados por coma)</param>
    /// <param name="alls">Si se deben obtener todos los tipos de centro</param>
    /// <returns>Los tipos de centro encontrados</returns>
    public async Task<dynamic> GetAllCenterTypes(int take, int skip, string name, bool alls, bool isList)
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
                string cacheKey = string.Format(_appSettings.Cache.Keys.CenterTypes, take, skip, name, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await db.QueryMultipleAsync("100_GetAllCenterType", parameters, commandType: CommandType.StoredProcedure);
                        var data = await result.ReadAsync<DTOCenterType>();
                        var count = await result.ReadSingleAsync<int>();
                        return new { data, count };
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(5)
                );
            }
            else
            {
                var result = await db.QueryMultipleAsync("100_GetAllCenterType", parameters, commandType: CommandType.StoredProcedure);
                var data = await result.ReadAsync<DTOCenterType>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de centro");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo tipo de centro
    /// </summary>
    /// <param name="request">El tipo de centro a insertar</param>
    /// <returns>True si la inserción fue exitosa, false en caso contrario</returns>
    public async Task<bool> InsertCenterType(CenterTypeRequest request)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", request.Name);
            parameters.Add("@nameEN", request.NameEN);
            parameters.Add("@displayOrder", request.DisplayOrder);
            parameters.Add("@isActive", request.IsActive);
            await db.ExecuteAsync("100_InsertCenterType", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de centro");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de centro existente
    /// </summary>
    /// <param name="request">El tipo de centro a actualizar</param>
    /// <returns>True si la actualización fue exitosa, false en caso contrario</returns>
    public async Task<bool> UpdateCenterType(DTOCenterType request)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", request.Id);
            parameters.Add("@name", request.Name);
            parameters.Add("@nameEN", request.NameEN);
            parameters.Add("@displayOrder", request.DisplayOrder);
            parameters.Add("@isActive", request.IsActive);
            var rows = await db.ExecuteAsync("100_UpdateCenterType", parameters, commandType: CommandType.StoredProcedure);
            InvalidateCache(request.Id);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de centro");
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de centro existente
    /// </summary>
    /// <param name="id">El ID del tipo de centro a eliminar</param>
    /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
    public async Task<bool> DeleteCenterType(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id);
            var rows = await db.ExecuteAsync("100_DeleteCenterType", param, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de centro");
            throw;
        }
    }

    private void InvalidateCache(int? centerTypeId = null)
    {
        if (centerTypeId.HasValue)
        {
            _cache.Remove($"CenterType_{centerTypeId}");
        }
        _cache.Remove("CenterTypes");
        _logger.LogInformation("Cache invalidado para CenterType Repository");
    }
}