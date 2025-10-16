-- =============================================
-- Script: Setup School Permissions Complete
-- Descripción: Script maestro para configurar completamente los permisos de escuela
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

PRINT 'Starting School Permissions Setup...';
PRINT '=====================================';

-- Paso 1: Insertar los permisos de escuela
PRINT 'Step 1: Inserting school permissions...';
EXEC('
-- Insertar permisos para escuelas (solo si no existen)
IF NOT EXISTS (SELECT 1 FROM Permission WHERE ValueKey = ''school.view'')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), ''school.view'', ''Ver escuelas'', ''View schools'', 1, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE ValueKey = ''school.create'')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), ''school.create'', ''Crear escuelas'', ''Create schools'', 1, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE ValueKey = ''school.edit'')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), ''school.edit'', ''Editar escuelas'', ''Edit schools'', 1, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE ValueKey = ''school.delete'')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), ''school.delete'', ''Eliminar escuelas'', ''Delete schools'', 1, GETDATE(), GETDATE());
END
');

PRINT 'Step 1 completed: School permissions inserted';

-- Paso 2: Asignar permisos al rol Agency-Administrator
PRINT 'Step 2: Assigning school permissions to Agency-Administrator role...';

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
END

PRINT 'Step 2 completed: School permissions assigned to Agency-Administrator role';

-- Paso 3: Verificar configuración
PRINT 'Step 3: Verifying configuration...';

SELECT
    p.ValueKey,
    p.Name,
    r.Name as RoleName
FROM Permission p
    INNER JOIN RolePermission rp ON p.Id = rp.PermissionId
    INNER JOIN AspNetRoles r ON rp.RoleId = r.Id
WHERE p.ValueKey LIKE 'school.%'
ORDER BY p.ValueKey;

PRINT '=====================================';
PRINT 'School Permissions Setup Completed Successfully!';
PRINT '=====================================';
