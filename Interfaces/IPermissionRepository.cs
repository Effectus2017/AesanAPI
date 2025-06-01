using Api.Models;

namespace Api.Interfaces;

public interface IPermissionRepository
{
    Task<dynamic> GetPermissionById(int id);
    Task<dynamic> GetAllPermissions(int take, int skip, string name, bool alls);
    Task<dynamic> GetUserPermissions(string userId);
    Task<dynamic> GetRolePermissions(string roleId);
    Task<bool> InsertPermission(DTOPermission permission);
    Task<bool> UpdatePermission(DTOPermission permission);
    Task<bool> DeletePermission(int id);
    // Métodos para asignación de permisos a usuarios y roles
    Task<bool> AssignPermissionToUser(string userId, int permissionId);
    Task<bool> RemovePermissionFromUser(string userId, int permissionId);
    Task<bool> AssignPermissionToRole(string roleId, int permissionId);
    Task<bool> RemovePermissionFromRole(string roleId, int permissionId);

}