using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class CenterTypeRepository(DapperContext context, ILogger<CenterTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : ICenterTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<CenterTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

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
    public async Task<dynamic> GetAllCenterTypes(int take, int skip, string name, bool alls, bool forDropdown)
    {
        try
        {

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            if (forDropdown)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.CenterTypes, take, skip, name, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        using var result = await db.QueryMultipleAsync("100_GetAllCenterType", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(_mappingService.MapCenterTypeList).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                var result = await db.QueryMultipleAsync("100_GetAllCenterType", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null!;
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapCenterType).ToList();
                var count = result.ReadFirstOrDefault<int>();
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
    /// <returns>El ID del tipo de centro insertado, o 0 si falló</returns>
    public async Task<int> InsertCenterType(CenterTypeRequest request)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", request.Name);
            parameters.Add("@nameEN", request.NameEN);
            parameters.Add("@displayOrder", request.DisplayOrder);
            parameters.Add("@isActive", request.IsActive);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertCenterType", parameters, commandType: CommandType.StoredProcedure);

            var newId = parameters.Get<int>("@id");
            InvalidateCache();
            return newId;
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

            if (rows > 0)
            {
                InvalidateCache(id);
            }

            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de centro");
            throw;
        }
    }

    /// <summary>
    /// Obtiene los tipos de centro válidos para un programa específico
    /// </summary>
    /// <param name="programId">El ID del programa</param>
    /// <returns>Los tipos de centro válidos para el programa</returns>
    public async Task<dynamic> GetCenterTypesByProgram(int programId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@programId", programId, DbType.Int32);

            var result = await db.QueryAsync<DTOCenterType>("100_GetCenterTypesByProgram", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de centro para el programa {ProgramId}", programId);
            throw;
        }
    }

    private void InvalidateCache(int? centerTypeId = null)
    {
        try
        {
            // Invalidar cache específico del tipo de centro si se proporciona ID
            if (centerTypeId.HasValue)
            {
                _cache.Remove($"CenterType_{centerTypeId}");
            }

            // Invalidar todos los caches relacionados con CenterTypes
            // Buscar y remover todas las claves que contengan "CenterTypes"
            var cacheKeys = _cache.GetType()
                .GetField("_entries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .GetValue(_cache) as System.Collections.IDictionary;

            if (cacheKeys != null)
            {
                var keysToRemove = new List<object>();
                foreach (var key in cacheKeys.Keys)
                {
                    if (key?.ToString()?.Contains("CenterTypes") == true)
                    {
                        keysToRemove.Add(key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                }
            }

            _logger.LogInformation("Cache invalidado para CenterType Repository");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al invalidar cache de CenterType");
        }
    }

}