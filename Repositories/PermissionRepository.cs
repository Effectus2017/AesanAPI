using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class PermissionRepository(DapperContext context, ILogger<PermissionRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IPermissionRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<PermissionRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un permiso por su ID
    /// </summary>
    /// <param name="id">ID del permiso a buscar</param>
    /// <returns>El permiso encontrado o null si no existe</returns>
    public async Task<dynamic> GetPermissionById(int id)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        var result = await db.QueryMultipleAsync("100_GetPermissionById", parameters, commandType: CommandType.StoredProcedure);
        var data = await result.ReadSingleOrDefaultAsync<DTOPermission>();
        return data;
    }

    /// <summary>
    /// Obtiene todos los permisos con paginación y filtrado opcional
    /// </summary>
    /// <param name="take">Cantidad de registros a tomar</param>
    /// <param name="skip">Cantidad de registros a saltar</param>
    /// <param name="name">Nombre para filtrar (opcional)</param>
    /// <param name="alls">Indica si se deben obtener todos los registros sin paginar</param>
    /// <returns>Objeto con lista de permisos y conteo total</returns>
    public async Task<dynamic> GetAllPermissions(int take, int skip, string name, bool alls)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@take", take, DbType.Int32);
        parameters.Add("@skip", skip, DbType.Int32);
        parameters.Add("@name", name, DbType.String);
        parameters.Add("@alls", alls, DbType.Boolean);
        var result = await db.QueryMultipleAsync("100_GetAllPermissions", parameters, commandType: CommandType.StoredProcedure);
        var data = await result.ReadAsync<DTOPermission>();
        var count = await result.ReadSingleAsync<int>();
        return new { data, count };
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
            parameters.Add("@userId", userId, DbType.String);
            var result = await db.QueryAsync<DTOPermission>("100_GetUserPermissions", parameters, commandType: CommandType.StoredProcedure);
            return new { data = result, count = result.Count() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los permisos del usuario");
            throw;
        }
    }

    /// <summary>
    /// Obtiene los permisos de un rol
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <returns>Lista de permisos del rol</returns>
    public async Task<dynamic> GetRolePermissions(string roleId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            var result = await db.QueryAsync<DTOPermission>("100_GetRolePermissions", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los permisos del rol");
            throw;
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
            parameters.Add("@name", permission.Name, DbType.String);
            parameters.Add("@description", permission.Description, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertPermission", parameters, commandType: CommandType.StoredProcedure);

            var id = parameters.Get<int>("@id");

            if (id > 0)
            {
                InvalidateCache(id);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el permiso");
            throw;
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
            parameters.Add("@id", permission.Id, DbType.Int32);
            parameters.Add("@name", permission.Name, DbType.String);
            parameters.Add("@description", permission.Description, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdatePermission", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(permission.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el permiso");
            throw;
        }
    }

    /// <summary>
    /// Elimina un permiso por su ID
    /// </summary>
    /// <param name="id">ID del permiso a eliminar</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario</returns>
    public async Task<bool> DeletePermission(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
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
            _logger.LogError(ex, "Error al eliminar el permiso");
            throw;
        }
    }

    /// <summary>
    /// Asigna un permiso a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="permissionId">ID del permiso a asignar</param>
    /// <returns>True si la asignación fue exitosa, False en caso contrario</returns>
    public async Task<bool> AssignPermissionToUser(string userId, int permissionId)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.String);
        parameters.Add("@permissionId", permissionId, DbType.Int32);
        parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await db.ExecuteAsync("100_AssignPermissionToUser", parameters, commandType: CommandType.StoredProcedure);
        var id = parameters.Get<int>("@id");

        if (id > 0)
        {
            InvalidateCache(id);
        }

        return id > 0;
    }

    /// <summary>
    /// Remueve un permiso de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="permissionId">ID del permiso a remover</param>
    /// <returns>True si la remoción fue exitosa, False en caso contrario</returns>
    public async Task<bool> RemovePermissionFromUser(string userId, int permissionId)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.String);
        parameters.Add("@permissionId", permissionId, DbType.Int32);
        parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await db.ExecuteAsync("100_RemovePermissionFromUser", parameters, commandType: CommandType.StoredProcedure);
        var rowsAffected = parameters.Get<int>("@rowsAffected");

        if (rowsAffected > 0)
        {
            InvalidateCache(permissionId);
        }

        return rowsAffected > 0;
    }

    /// <summary>
    /// Asigna un permiso a un rol
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <param name="permissionId">ID del permiso a asignar</param>
    /// <returns>True si la asignación fue exitosa, False en caso contrario</returns>
    public async Task<bool> AssignPermissionToRole(string roleId, int permissionId)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@permissionId", permissionId, DbType.Int32);
        parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await db.ExecuteAsync("100_AssignPermissionToRole", parameters, commandType: CommandType.StoredProcedure);
        var id = parameters.Get<int>("@id");

        if (id > 0)
        {
            InvalidateCache(id);
        }

        return id > 0;
    }

    /// <summary>
    /// Remueve un permiso de un rol
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <param name="permissionId">ID del permiso a remover</param>
    /// <returns>True si la remoción fue exitosa, False en caso contrario</returns>
    public async Task<bool> RemovePermissionFromRole(string roleId, int permissionId)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@permissionId", permissionId, DbType.Int32);
        parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await db.ExecuteAsync("100_RemovePermissionFromRole", parameters, commandType: CommandType.StoredProcedure);
        var rowsAffected = parameters.Get<int>("@rowsAffected");

        if (rowsAffected > 0)
        {
            InvalidateCache(permissionId);
        }

        return rowsAffected > 0;
    }



    private void InvalidateCache(int? permissionId = null)
    {
        if (permissionId.HasValue)
        {
            _cache.Remove($"Permission_{permissionId}");
        }
        _cache.Remove("Permissions");
        _logger.LogInformation("Cache invalidado para Permission Repository");
    }
}