-- 1.0.0
-- Asigna permisos CRUD de escuelas a un usuario
CREATE OR ALTER PROCEDURE [100_AssignSchoolCrudPermissionsToUser]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener los IDs de los permisos CRUD de escuelas
    DECLARE @SchoolViewId INT, @SchoolCreateId INT, @SchoolEditId INT, @SchoolDeleteId INT;

    SELECT @SchoolViewId = Id
    FROM Permission
    WHERE Name = 'SchoolView';
    SELECT @SchoolCreateId = Id
    FROM Permission
    WHERE Name = 'SchoolCreate';
    SELECT @SchoolEditId = Id
    FROM Permission
    WHERE Name = 'SchoolEdit';
    SELECT @SchoolDeleteId = Id
    FROM Permission
    WHERE Name = 'SchoolDelete';

    -- Insertar los permisos si no existen ya para el usuario
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SchoolViewId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SchoolViewId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SchoolCreateId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SchoolCreateId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SchoolEditId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SchoolEditId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @SchoolDeleteId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @SchoolDeleteId);
END
GO