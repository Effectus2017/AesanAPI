-- 100_GetRolePermissions.sql
-- Obtiene todos los permisos asignados a un rol
CREATE OR ALTER PROCEDURE [100_GetRolePermissions]
    @roleId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.Id, p.Name, p.Description
    FROM Permission p
        INNER JOIN RolePermission rp ON p.Id = rp.PermissionId
    WHERE rp.RoleId = @roleId;
END; 