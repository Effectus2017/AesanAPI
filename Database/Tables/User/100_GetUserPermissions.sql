-- 1.0.1
CREATE OR ALTER PROCEDURE [100_GetUserPermissions]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Permisos directos al usuario
    SELECT DISTINCT p.Name AS PermissionName
    FROM UserPermission up
        INNER JOIN Permission p ON up.PermissionId = p.Id
    WHERE up.UserId = @userId
END
GO 