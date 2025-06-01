-- 100_GetUserPermissions.sql
-- Obtiene todos los permisos asignados a un usuario
CREATE OR ALTER PROCEDURE [100_GetUserPermissions]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.Id, p.Name, p.Description
    FROM Permission p
        INNER JOIN UserPermission up ON p.Id = up.PermissionId
    WHERE up.UserId = @userId;
END; 