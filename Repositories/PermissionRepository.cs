using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.Errors;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class PermissionRepository(DapperContext context, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IPermissionRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un permiso por su ValueKey
    /// </summary>
    /// <param name="valueKey">ValueKey del permiso a buscar</param>
    /// <returns>El permiso encontrado o null si no existe</returns>
    public async Task<dynamic> GetPermissionByValueKey(string valueKey)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@valueKey", valueKey, DbType.String, size: 50);
            var result = await db.QueryAsync<DTOPermission>("100_GetPermissionByValueKey", parameters, commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener el permiso por ValueKey", ex);
        }
    }

    /// <summary>
    /// Obtiene un permiso por su ID
    /// </summary>
    /// <param name="id">ID del permiso a buscar</param>
    /// <returns>El permiso encontrado o null si no existe</returns>
    public async Task<dynamic> GetPermissionById(string id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.String, size: 36);
            var result = await db.QueryMultipleAsync("100_GetPermissionById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleOrDefaultAsync<DTOPermission>();
            return data;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener el permiso por ID", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los permisos con paginación y filtrado opcional
    /// </summary>
    /// <param name="take">Cantidad de registros a tomar</param>
    /// <param name="skip">Cantidad de registros a saltar</param>
    /// <param name="valueKey">ValueKey para filtrar (opcional)</param>
    /// <param name="name">Nombre para filtrar (opcional)</param>
    /// <param name="alls">Indica si se deben obtener todos los registros sin paginar</param>
    /// <returns>Objeto con lista de permisos y conteo total</returns>
    public async Task<dynamic> GetAllPermissions(int take, int skip, string valueKey, string name, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@valueKey", valueKey, DbType.String, size: 50);
            parameters.Add("@name", name, DbType.String, size: 255);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllPermissions", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOPermission>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener todos los permisos", ex);
        }
    }

    /// <summary>
    /// Obtiene los permisos de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de permisos del usuario</returns>
    public async Task<dynamic> GetUserPermissions(string userId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String, size: 450);
            var result = await db.QueryAsync<DTOPermission>("100_GetUserPermissions", parameters, commandType: CommandType.StoredProcedure);
            return new { data = result, count = result.Count() };
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener los permisos del usuario", ex);
        }
    }

    /// <summary>
    /// Inserta un nuevo permiso
    /// </summary>
    /// <param name="permission">DTO con los datos del permiso a insertar</param>
    /// <returns>True si la inserción fue exitosa, False en caso contrario</returns>
    public async Task<bool> InsertPermission(DTOPermission permission)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@valueKey", permission.ValueKey, DbType.String, size: 50);
            parameters.Add("@name", permission.Name, DbType.String, size: 255);
            parameters.Add("@nameEn", permission.NameEn, DbType.String, size: 255);
            parameters.Add("@isActive", permission.IsActive, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.String, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertPermission", parameters, commandType: CommandType.StoredProcedure);

            var id = parameters.Get<string>("@id");

            if (!string.IsNullOrEmpty(id))
            {
                InvalidateCache(id);
            }

            return !string.IsNullOrEmpty(id);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al insertar el permiso", ex);
        }
    }

    /// <summary>
    /// Actualiza un permiso existente
    /// </summary>
    /// <param name="permission">DTO con los datos actualizados del permiso</param>
    /// <returns>True si la actualización fue exitosa, False en caso contrario</returns>
    public async Task<bool> UpdatePermission(DTOPermission permission)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", permission.Id, DbType.String);
            parameters.Add("@valueKey", permission.ValueKey, DbType.String, size: 50);
            parameters.Add("@name", permission.Name, DbType.String, size: 255);
            parameters.Add("@nameEn", permission.NameEn, DbType.String, size: 255);
            parameters.Add("@isActive", permission.IsActive, DbType.Boolean);
            var rowsAffected = await db.ExecuteAsync("100_UpdatePermission", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache(permission.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al actualizar el permiso", ex);
        }
    }

    /// <summary>
    /// Elimina un permiso por su ID
    /// </summary>
    /// <param name="id">ID del permiso a eliminar</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
    public async Task<bool> DeletePermission(string id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.String, size: 36);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeletePermission", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al eliminar el permiso", ex);
        }
    }

    /// <summary>
    /// Asigna un permiso a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="permissionId">ID del permiso a asignar</param>
    /// <returns>True si la asignación fue exitosa, False en caso contrario</returns>
    public async Task<bool> AssignPermissionToUser(string userId, string permissionId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String, size: 450);
            parameters.Add("@permissionId", permissionId, DbType.String, size: 36);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_AssignPermissionToUser", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(permissionId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al asignar permiso al usuario", ex);
        }
    }

    /// <summary>
    /// Remueve un permiso de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="permissionId">ID del permiso a remover</param>
    /// <returns>True si la remoción fue exitosa, False en caso contrario</returns>
    public async Task<bool> RemovePermissionFromUser(string userId, string permissionId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String, size: 450);
            parameters.Add("@permissionId", permissionId, DbType.String, size: 36);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_RemovePermissionFromUser", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(permissionId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al remover permiso del usuario", ex);
        }
    }

    /// <summary>
    /// Asigna un permiso a un rol
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <param name="permissionId">ID del permiso a asignar</param>
    /// <returns>True si la asignación fue exitosa, False en caso contrario</returns>
    public async Task<bool> AssignPermissionToRole(string roleId, string permissionId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String, size: 450);
            parameters.Add("@permissionId", permissionId, DbType.String, size: 36);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_AssignPermissionToRole", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(permissionId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al asignar permiso al rol", ex);
        }
    }

    /// <summary>
    /// Remueve un permiso de un rol
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <param name="permissionId">ID del permiso a remover</param>
    /// <returns>True si la remoción fue exitosa, False en caso contrario</returns>
    public async Task<bool> RemovePermissionFromRole(string roleId, string permissionId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String, size: 450);
            parameters.Add("@permissionId", permissionId, DbType.String, size: 36);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_RemovePermissionFromRole", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(permissionId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al remover permiso del rol", ex);
        }
    }

    private void InvalidateCache(string? permissionId = null)
    {
        if (!string.IsNullOrEmpty(permissionId))
        {
            _cache.Remove($"Permission_{permissionId}");
        }
        _cache.Remove("Permissions");
    }
}