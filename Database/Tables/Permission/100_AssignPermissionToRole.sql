-- 100_AssignPermissionToRole.sql
-- Asigna un permiso a un rol
CREATE OR ALTER PROCEDURE [100_AssignPermissionToRole]
    @roleId NVARCHAR(450),
    @permissionId VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS ( SELECT 1
    FROM RolePermission
    WHERE RoleId = @roleId AND PermissionId = @permissionId
    )
    BEGIN
        INSERT INTO RolePermission
            (RoleId, PermissionId)
        VALUES
            (@roleId, @permissionId);
        RETURN @@ROWCOUNT;
    END
    ELSE
    BEGIN
        RETURN 0;
    END
END; 