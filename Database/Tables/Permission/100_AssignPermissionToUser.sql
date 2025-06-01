-- 100_AssignPermissionToUser.sql
-- Asigna un permiso a un usuario
CREATE OR ALTER PROCEDURE [100_AssignPermissionToUser]
    @userId NVARCHAR(450),
    @permissionId INT
AS
BEGIN
    DECLARE @id INT;
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
        SET @id = SCOPE_IDENTITY();
        RETURN @id;
    END
    ELSE
    BEGIN
        SET @id = 0;
        RETURN @id;
    END
END; 