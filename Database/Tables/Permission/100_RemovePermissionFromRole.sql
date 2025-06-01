-- 100_RemovePermissionFromRole.sql
-- Remueve un permiso de un rol
CREATE OR ALTER PROCEDURE [100_RemovePermissionFromRole]
    @roleId NVARCHAR(450),
    @permissionId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;
    DELETE FROM RolePermission WHERE RoleId = @roleId AND PermissionId = @permissionId;
    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END; 