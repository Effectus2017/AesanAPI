-- =============================================
-- Script: Assign School Permissions to Agency-Administrator Role
-- Descripción: Asigna los permisos CRUD de escuelas al rol Agency-Administrator
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

DECLARE @AgencyAdministratorRoleId NVARCHAR(450);
DECLARE @SchoolViewPermissionId VARCHAR(36);
DECLARE @SchoolCreatePermissionId VARCHAR(36);
DECLARE @SchoolEditPermissionId VARCHAR(36);
DECLARE @SchoolDeletePermissionId VARCHAR(36);

-- Obtener el ID del rol Agency-Administrator
SELECT @AgencyAdministratorRoleId = Id
FROM AspNetRoles
WHERE Name = 'Agency-Administrator';

-- Verificar que el rol existe
IF @AgencyAdministratorRoleId IS NULL
BEGIN
    PRINT 'ERROR: Agency-Administrator role not found';
    RETURN;
END

-- Obtener los IDs de los permisos de escuela
SELECT @SchoolViewPermissionId = Id
FROM Permission
WHERE ValueKey = 'school.view';

SELECT @SchoolCreatePermissionId = Id
FROM Permission
WHERE ValueKey = 'school.create';

SELECT @SchoolEditPermissionId = Id
FROM Permission
WHERE ValueKey = 'school.edit';

SELECT @SchoolDeletePermissionId = Id
FROM Permission
WHERE ValueKey = 'school.delete';

-- Asignar permisos al rol (solo si no existen ya)
IF @SchoolViewPermissionId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1
    FROM RolePermission
    WHERE RoleId = @AgencyAdministratorRoleId AND PermissionId = @SchoolViewPermissionId)
    BEGIN
        INSERT INTO RolePermission
            (RoleId, PermissionId)
        VALUES
            (@AgencyAdministratorRoleId, @SchoolViewPermissionId);
        PRINT 'Assigned school.view permission to Agency-Administrator role';
    END
    ELSE
    BEGIN
        PRINT 'school.view permission already assigned to Agency-Administrator role';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: school.view permission not found';
END

IF @SchoolCreatePermissionId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1
    FROM RolePermission
    WHERE RoleId = @AgencyAdministratorRoleId AND PermissionId = @SchoolCreatePermissionId)
    BEGIN
        INSERT INTO RolePermission
            (RoleId, PermissionId)
        VALUES
            (@AgencyAdministratorRoleId, @SchoolCreatePermissionId);
        PRINT 'Assigned school.create permission to Agency-Administrator role';
    END
    ELSE
    BEGIN
        PRINT 'school.create permission already assigned to Agency-Administrator role';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: school.create permission not found';
END

IF @SchoolEditPermissionId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1
    FROM RolePermission
    WHERE RoleId = @AgencyAdministratorRoleId AND PermissionId = @SchoolEditPermissionId)
    BEGIN
        INSERT INTO RolePermission
            (RoleId, PermissionId)
        VALUES
            (@AgencyAdministratorRoleId, @SchoolEditPermissionId);
        PRINT 'Assigned school.edit permission to Agency-Administrator role';
    END
    ELSE
    BEGIN
        PRINT 'school.edit permission already assigned to Agency-Administrator role';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: school.edit permission not found';
END

IF @SchoolDeletePermissionId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1
    FROM RolePermission
    WHERE RoleId = @AgencyAdministratorRoleId AND PermissionId = @SchoolDeletePermissionId)
    BEGIN
        INSERT INTO RolePermission
            (RoleId, PermissionId)
        VALUES
            (@AgencyAdministratorRoleId, @SchoolDeletePermissionId);
        PRINT 'Assigned school.delete permission to Agency-Administrator role';
    END
    ELSE
    BEGIN
        PRINT 'school.delete permission already assigned to Agency-Administrator role';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: school.delete permission not found';
END

PRINT 'School permissions assignment completed';
