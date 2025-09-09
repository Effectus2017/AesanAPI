-- 100_RemovePermissionFromUser.sql
-- Remueve un permiso de un usuario
CREATE OR ALTER PROCEDURE [100_RemovePermissionFromUser]
    @userId NVARCHAR(450),
    @permissionId VARCHAR(36),
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM UserPermission WHERE UserId = @userId AND PermissionId = @permissionId;
    SET @rowsAffected = @@ROWCOUNT;
END; 