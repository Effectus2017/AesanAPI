-- 1.0.0
-- Asigna permisos CRUD de staff a un usuario
CREATE OR ALTER PROCEDURE [100_AssignStaffCrudPermissionsToUser]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener los IDs de los permisos CRUD de staff
    DECLARE @StaffViewId VARCHAR(36), @StaffCreateId VARCHAR(36), @StaffEditId VARCHAR(36), @StaffDeleteId VARCHAR(36);

    SELECT @StaffViewId = Id
    FROM Permission
    WHERE ValueKey = 'staff.view';
    SELECT @StaffCreateId = Id
    FROM Permission
    WHERE ValueKey = 'staff.create';
    SELECT @StaffEditId = Id
    FROM Permission
    WHERE ValueKey = 'staff.edit';
    SELECT @StaffDeleteId = Id
    FROM Permission
    WHERE ValueKey = 'staff.delete';

    -- Insertar los permisos si no existen ya para el usuario
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @StaffViewId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @StaffViewId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @StaffCreateId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @StaffCreateId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @StaffEditId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @StaffEditId);
    IF NOT EXISTS (SELECT 1
    FROM UserPermission
    WHERE UserId = @userId AND PermissionId = @StaffDeleteId)
        INSERT INTO UserPermission
        (UserId, PermissionId)
    VALUES
        (@userId, @StaffDeleteId);
END
GO
