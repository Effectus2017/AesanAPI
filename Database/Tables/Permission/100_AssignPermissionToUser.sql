-- 100_AssignPermissionToUser.sql
-- Asigna un permiso a un usuario
CREATE OR ALTER PROCEDURE [100_AssignPermissionToUser]
    @userId NVARCHAR(450),
    @permissionId VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @permissionId
    )
    BEGIN
        INSERT INTO UserPermission
            (UserId, PermissionId)
        VALUES
            (@userId, @permissionId);
        RETURN @@ROWCOUNT;
    END
    ELSE
    BEGIN
        RETURN 0;
    END
END; 