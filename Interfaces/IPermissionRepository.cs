using Api.Models;

namespace Api.Interfaces;

public interface IPermissionRepository
{
    Task<dynamic> GetPermissionByValueKey(string valueKey);
    Task<dynamic> GetPermissionById(string id);
    Task<dynamic> GetAllPermissions(int take, int skip, string valueKey, string name, bool alls);
    Task<dynamic> GetUserPermissions(string userId);
    Task<bool> InsertPermission(DTOPermission permission);
    Task<bool> UpdatePermission(DTOPermission permission);
    Task<bool> DeletePermission(string id);
    // Métodos para asignación de permisos a usuarios
    Task<bool> AssignPermissionToUser(string userId, string permissionId);
    Task<bool> RemovePermissionFromUser(string userId, string permissionId);
}