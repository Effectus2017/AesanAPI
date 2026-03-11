using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Api.Models.Errors;

namespace Api.Repositories;

/// <summary>
/// Implementación del repositorio para relaciones entre empleados
/// </summary>
public class StaffRelationshipRepository(
    DapperContext context,
    ILogger<StaffRelationshipRepository> logger,
    IMemoryCache cache,
    IOptions<ApplicationSettings> appSettings,
    MappingService mappingService) : IStaffRelationshipRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<StaffRelationshipRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene todas las relaciones de un empleado específico
    /// </summary>
    /// <param name="staffId">ID del empleado</param>
    /// <returns>Lista de relaciones del empleado</returns>
    public async Task<dynamic> GetRelationshipsByStaffId(int staffId, bool isActive)
    {
        try
        {
            _logger.LogInformation("Obteniendo relaciones del empleado con ID {StaffId}", staffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@staffId", staffId, DbType.Int32);
            param.Add("@isActive", isActive, DbType.Boolean); // 1 = activo, 0 = inactivo

            var result = await dbConnection.QueryAsync<dynamic>("100_GetStaffRelationships", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new List<dynamic>();
            }

            var data = result.Select(_mappingService.MapStaffRelationship).ToList();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las relaciones del empleado con ID {StaffId}", staffId);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Obtiene todas las relaciones activas de la agencia
    /// </summary>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="alls">Si se deben obtener todos los registros</param>
    /// <param name="forDropdown">Si es para lista simple (dropdown), devuelve solo datos; si no, devuelve { data, count }</param>
    /// <returns>Lista de todas las relaciones activas</returns>
    public async Task<dynamic> GetAllActiveRelationshipsFromDb(int take, int skip, bool alls, bool forDropdown)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las relaciones activas");

            string cacheKey = $"StaffRelationship_AllActive_{take}_{skip}_{alls}_{forDropdown}";

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@alls", alls, DbType.Boolean);

            if (forDropdown)
            {
                return await _cache.CacheQuery<dynamic>(
                    cacheKey,
                    async () =>
                    {
                        var result = await dbConnection.QueryMultipleAsync("100_GetAllActiveStaffRelationships", param, commandType: CommandType.StoredProcedure);
                        if (result == null)
                        {
                            return new List<dynamic>();
                        }
                        _ = result.ReadFirstOrDefault<int>(); // TotalCount, primer result set
                        var data = result.Read<dynamic>().Select(_mappingService.MapStaffRelationship).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync("100_GetAllActiveStaffRelationships", param, commandType: CommandType.StoredProcedure);
                if (result == null)
                {
                    return null;
                }
                var count = result.ReadFirstOrDefault<int>();
                var data = result.Read<dynamic>().Select(_mappingService.MapStaffRelationship).ToList();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las relaciones activas");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }


    /// <summary>
    /// Obtiene una relación específica por su ID
    /// </summary>
    /// <param name="id">ID de la relación</param>
    /// <returns>Relación encontrada o null</returns>
    public async Task<dynamic> GetRelationshipById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo relación con ID {Id}", id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetRelationshipById", param, commandType: CommandType.StoredProcedure);

            if (result == null) return null;

            return _mappingService.MapStaffRelationship(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la relación con ID {Id}", id);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Crea una nueva relación entre empleados
    /// </summary>
    /// <param name="request">Datos de la relación a crear</param>
    /// <returns>ID de la relación creada</returns>
    public async Task<int> CreateRelationship(StaffRelationshipRequest request)
    {
        try
        {
            _logger.LogInformation("Creando nueva relación entre empleados {StaffId} y {RelatedStaffId}", request.StaffId, request.RelatedStaffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@staffId", request.StaffId, DbType.Int32);
            param.Add("@relatedStaffId", request.RelatedStaffId, DbType.Int32);
            param.Add("@relationshipTypeId", request.RelationshipTypeId, DbType.Int32);
            param.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertStaffRelationship", param, commandType: CommandType.StoredProcedure);

            var relationshipId = param.Get<int>("@id");

            if (relationshipId > 0)
            {
                InvalidateCache(request.StaffId);
                InvalidateCache(request.RelatedStaffId);
            }

            return relationshipId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la relación entre empleados {StaffId} y {RelatedStaffId}", request.StaffId, request.RelatedStaffId);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Actualiza una relación existente
    /// </summary>
    /// <param name="request">Datos de la relación a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateRelationship(UpdateStaffRelationshipRequest request)
    {
        try
        {
            _logger.LogInformation("Actualizando relación con ID {Id}", request.Id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", request.Id, DbType.Int32);
            param.Add("@relationshipTypeId", request.RelationshipTypeId, DbType.Int32);
            param.Add("@isActive", request.IsActive, DbType.Boolean);
            param.Add("@comment", request.Comment, DbType.String);
            param.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_UpdateStaffRelationship", param, commandType: CommandType.StoredProcedure);

            var rowsAffected = param.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidate cache for related staff members
                var relationship = await GetRelationshipById(request.Id);
                if (relationship != null)
                {
                    InvalidateCache(relationship.Staff.Id);
                    InvalidateCache(relationship.RelatedStaff.Id);
                }
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la relación con ID {Id}", request.Id);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Desactiva una relación (soft delete)
    /// </summary>
    /// <param name="id">ID de la relación a desactivar</param>
    /// <returns>True si se desactivó correctamente</returns>
    public async Task<bool> DeleteRelationship(int id)
    {
        try
        {
            _logger.LogInformation("Desactivando relación con ID {Id}", id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);
            param.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_DeleteStaffRelationship", param, commandType: CommandType.StoredProcedure);

            var rowsAffected = param.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidate cache for related staff members
                var relationship = await GetRelationshipById(id);
                if (relationship != null)
                {
                    InvalidateCache(relationship.Staff.Id);
                    InvalidateCache(relationship.RelatedStaff.Id);
                }
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar la relación con ID {Id}", id);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Verifica si existe una relación activa entre dos empleados
    /// </summary>
    /// <param name="staffId">ID del primer empleado</param>
    /// <param name="relatedStaffId">ID del segundo empleado</param>
    /// <returns>True si existe una relación activa</returns>
    public async Task<bool> RelationshipExists(int staffId, int relatedStaffId)
    {
        try
        {
            _logger.LogInformation("Verificando existencia de relación entre empleados {StaffId} y {RelatedStaffId}", staffId, relatedStaffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@staffId", staffId, DbType.Int32);
            param.Add("@relatedStaffId", relatedStaffId, DbType.Int32);
            param.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_RelationshipExists", param, commandType: CommandType.StoredProcedure);

            var rowsAffected = param.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe relación entre empleados {StaffId} y {RelatedStaffId}", staffId, relatedStaffId);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }


    /// <summary>
    /// Obtiene las relaciones por tipo específico
    /// </summary>
    /// <param name="relationshipTypeId">ID del tipo de parentesco</param>
    /// <returns>Lista de relaciones del tipo especificado</returns>
    public async Task<dynamic> GetRelationshipsByType(int relationshipTypeId)
    {
        try
        {
            _logger.LogInformation("Obteniendo relaciones por tipo {RelationshipTypeId}", relationshipTypeId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@relationshipTypeId", relationshipTypeId, DbType.Int32);

            var result = await dbConnection.QueryAsync<dynamic>("100_GetRelationshipsByType", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new List<dynamic>();
            }

            var data = result.Select(_mappingService.MapStaffRelationship).ToList();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las relaciones por tipo {RelationshipTypeId}", relationshipTypeId);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Verifica si un empleado puede tener el tipo de relación especificado
    /// </summary>
    /// <param name="staffId">ID del empleado</param>
    /// <param name="relationshipTypeId">ID del tipo de parentesco</param>
    /// <param name="excludeRelationshipId">ID de la relación a excluir (útil para ediciones)</param>
    /// <returns>True si puede tener ese tipo de relación</returns>
    public async Task<bool> CanHaveRelationshipType(int staffId, int relationshipTypeId, int? excludeRelationshipId = null)
    {
        try
        {
            _logger.LogInformation("Verificando si empleado {StaffId} puede tener tipo de relación {RelationshipTypeId}", staffId, relationshipTypeId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@staffId", staffId, DbType.Int32);
            param.Add("@relationshipTypeId", relationshipTypeId, DbType.Int32);
            param.Add("@excludeRelationshipId", excludeRelationshipId, DbType.Int32);
            param.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_CanHaveRelationshipType", param, commandType: CommandType.StoredProcedure);

            var rowsAffected = param.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si el empleado {StaffId} puede tener el tipo de relación {RelationshipTypeId}", staffId, relationshipTypeId);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }


    /// <summary>
    /// Invalida el caché para las relaciones de staff
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    private void InvalidateCache(int staffId)
    {
        try
        {
            _cache.Remove("StaffRelationship_AllActive");

            // También invalidar caché de staff general
            if (_appSettings.Cache.Keys.Staff != null)
            {
                _cache.Remove(string.Format(_appSettings.Cache.Keys.Staff, 0, 0, "", false, null));
                _cache.Remove(_appSettings.Cache.Keys.Staff);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al invalidar caché para staff con ID {StaffId}", staffId);
        }
    }
}
