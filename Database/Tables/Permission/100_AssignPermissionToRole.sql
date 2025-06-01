-- 100_AssignPermissionToRole.sql
-- Asigna un permiso a un rol
CREATE OR ALTER PROCEDURE [100_AssignPermissionToRole]
    @roleId NVARCHAR(450),
    @permissionId INT
AS
BEGIN
    DECLARE @id INT;
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
        SET @id = SCOPE_IDENTITY();
        RETURN @id;
    END
    ELSE
    BEGIN
        SET @id = 0;
        RETURN @id;
    END
END; 