-- 1.0.0
-- Asigna permisos CRUD de sitios a un usuario
CREATE OR ALTER PROCEDURE [100_AssignSiteCrudPermissionsToUser]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener los IDs de los permisos CRUD de sitios
    DECLARE @SiteViewId VARCHAR(36), @SiteCreateId VARCHAR(36), @SiteEditId VARCHAR(36), @SiteDeleteId VARCHAR(36);

    SELECT @SiteViewId = Id
    FROM Permission
    WHERE ValueKey = 'site.view';
    SELECT @SiteCreateId = Id
    FROM Permission
    WHERE ValueKey = 'site.create';
    SELECT @SiteEditId = Id
    FROM Permission
    WHERE ValueKey = 'site.edit';
    SELECT @SiteDeleteId = Id
    FROM Permission
    WHERE ValueKey = 'site.delete';

    -- Insertar los permisos si no existen ya para el usuario
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SiteViewId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SiteViewId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SiteCreateId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SiteCreateId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SiteEditId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SiteEditId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SiteDeleteId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SiteDeleteId);
END
GO

