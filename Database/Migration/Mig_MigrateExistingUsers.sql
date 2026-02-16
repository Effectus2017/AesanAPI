-- =====================================================
-- Script de Migración de Usuarios Existentes - AESAN
-- Fecha: 2025-01-15
-- Descripción: Migra usuarios existentes a los nuevos roles según criterios específicos
-- =====================================================

-- Verificar que las tablas existan
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'AspNetUsers')
BEGIN
    RAISERROR('La tabla AspNetUsers no existe. Ejecute primero el script de migración de roles.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'AspNetRoles')
BEGIN
    RAISERROR('La tabla AspNetRoles no existe. Ejecute primero el script de migración de roles.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'AspNetUserRoles')
BEGIN
    RAISERROR('La tabla AspNetUserRoles no existe. Ejecute primero el script de migración de roles.', 16, 1);
    RETURN;
END

PRINT 'Iniciando migración de usuarios existentes...';

BEGIN TRY
    BEGIN TRANSACTION;

    -- =====================================================
    -- PASO 1: Identificar usuarios con roles deprecados
    -- =====================================================
    
    PRINT 'Paso 1: Identificando usuarios con roles deprecados...';
    
    -- Usuarios con rol Monitor
    DECLARE @MonitorUsers TABLE (
    UserId NVARCHAR(450),
    UserName NVARCHAR(256),
    Email NVARCHAR(256),
    FirstName NVARCHAR(100),
    FatherLastName NVARCHAR(100)
    );
    
    INSERT INTO @MonitorUsers
SELECT
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.FatherLastName
FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Monitor' AND ur.IsActive = 1;
    
    DECLARE @MonitorCount INT = (SELECT COUNT(*)
FROM @MonitorUsers);
    PRINT '  - Usuarios con rol Monitor encontrados: ' + CAST(@MonitorCount AS VARCHAR(10));
    
    -- Usuarios con rol Agency-User
    DECLARE @AgencyUsers TABLE (
    UserId NVARCHAR(450),
    UserName NVARCHAR(256),
    Email NVARCHAR(256),
    FirstName NVARCHAR(100),
    FatherLastName NVARCHAR(100)
    );
    
    INSERT INTO @AgencyUsers
SELECT
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.FatherLastName
FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Agency-User' AND ur.IsActive = 1;
    
    DECLARE @AgencyUserCount INT = (SELECT COUNT(*)
FROM @AgencyUsers);
    PRINT '  - Usuarios con rol Agency-User encontrados: ' + CAST(@AgencyUserCount AS VARCHAR(10));

    -- =====================================================
    -- PASO 2: Crear tabla temporal para migración manual
    -- =====================================================
    
    PRINT 'Paso 2: Creando tabla temporal para migración manual...';
    
    -- Crear tabla temporal para usuarios que necesitan migración manual
    IF OBJECT_ID('tempdb..#UsersNeedingMigration') IS NOT NULL
        DROP TABLE #UsersNeedingMigration;
    
    CREATE TABLE #UsersNeedingMigration
(
    UserId NVARCHAR(450),
    CurrentRole NVARCHAR(256),
    SuggestedNewRole NVARCHAR(256),
    UserName NVARCHAR(256),
    Email NVARCHAR(256),
    FirstName NVARCHAR(100),
    FatherLastName NVARCHAR(100),
    MigrationReason NVARCHAR(500)
);
    
    -- Agregar usuarios Monitor (necesitan migración manual)
    INSERT INTO #UsersNeedingMigration
SELECT
    UserId,
    'Monitor' AS CurrentRole,
    'Por definir' AS SuggestedNewRole,
    UserName,
    Email,
    FirstName,
    FatherLastName,
    'Rol Monitor deprecado - necesita asignación manual a rol apropiado' AS MigrationReason
FROM @MonitorUsers;
    
    -- Agregar usuarios Agency-User (necesitan migración manual)
    INSERT INTO #UsersNeedingMigration
SELECT
    UserId,
    'Agency-User' AS CurrentRole,
    'Por definir' AS SuggestedNewRole,
    UserName,
    Email,
    FirstName,
    FatherLastName,
    'Rol Agency-User deprecado - migrar a Funcionario-Determinante, Funcionario-Confirmante o Funcionario-Revision-Independiente según función específica' AS MigrationReason
FROM @AgencyUsers;

    -- =====================================================
    -- PASO 3: Migración automática de usuarios específicos
    -- =====================================================
    
    PRINT 'Paso 3: Realizando migraciones automáticas...';
    
    -- Migrar usuarios específicos conocidos (ejemplo)
    -- NOTA: Estos son ejemplos - ajustar según usuarios reales del sistema
    
    -- Ejemplo: Migrar usuario admin@admin.com de Administrator a Administrador (ya actualizado en script de roles)
    -- Ejemplo: Migrar usuario superadmin@example.com de SuperAdministrator a Super-Administrador (ya actualizado en script de roles)
    -- Ejemplo: Migrar usuario user@example.com de Monitor a Funcionario Determinante (ejemplo)
    
    DECLARE @ExampleUserId NVARCHAR(450);
    SELECT @ExampleUserId = Id
FROM AspNetUsers
WHERE Email = 'user@example.com';
    
    IF @ExampleUserId IS NOT NULL
    BEGIN
    DECLARE @FuncDeterminantRoleId NVARCHAR(450);
    SELECT @FuncDeterminantRoleId = Id
    FROM AspNetRoles
    WHERE Name = 'Funcionario Determinante';

    IF @FuncDeterminantRoleId IS NOT NULL
        BEGIN
        -- Eliminar rol Monitor
        DELETE FROM AspNetUserRoles 
            WHERE UserId = @ExampleUserId
            AND RoleId IN (SELECT Id
            FROM AspNetRoles
            WHERE Name = 'Monitor');

        -- Asignar nuevo rol
        INSERT INTO AspNetUserRoles
            (UserId, RoleId, IsActive, CreatedAt)
        VALUES
            (@ExampleUserId, @FuncDeterminantRoleId, 1, GETDATE());

        PRINT '  - Usuario user@example.com migrado de Monitor a Funcionario Determinante';
    END
END

    -- =====================================================
    -- PASO 4: Mostrar usuarios que necesitan migración manual
    -- =====================================================
    
    PRINT 'Paso 4: Usuarios que requieren migración manual:';
    
    DECLARE @ManualMigrationCount INT = (SELECT COUNT(*)
FROM #UsersNeedingMigration);
    
    IF @ManualMigrationCount > 0
    BEGIN
    PRINT '=====================================================';
    PRINT 'USUARIOS QUE REQUIEREN MIGRACIÓN MANUAL:';
    PRINT '=====================================================';

    SELECT
        UserName,
        Email,
        FirstName + ' ' + FatherLastName AS NombreCompleto,
        CurrentRole,
        SuggestedNewRole,
        MigrationReason
    FROM #UsersNeedingMigration
    ORDER BY CurrentRole, UserName;

    PRINT '=====================================================';
    PRINT 'INSTRUCCIONES PARA MIGRACIÓN MANUAL:';
    PRINT '=====================================================';
    PRINT '1. Para usuarios con rol Monitor:';
    PRINT '   - Evaluar su función específica';
    PRINT '   - Asignar rol apropiado: Coordinador, Evaluador, Especialista, etc.';
    PRINT '';
    PRINT '2. Para usuarios con rol Agency-User:';
    PRINT '   - Evaluar su función específica en elegibilidad';
    PRINT '   - Asignar rol apropiado:';
    PRINT '     * Funcionario Determinante: Para determinar elegibilidad';
    PRINT '     * Funcionario Confirmante: Para confirmar elegibilidad';
    PRINT '     * Funcionario Revisión Independiente: Para revisión independiente';
    PRINT '';
    PRINT '3. Usar el siguiente script para migrar cada usuario:';
    PRINT '   EXEC [dbo].[MigrateUserRole] @UserId = ''USER_ID'', @NewRoleName = ''NUEVO_ROL''';
    PRINT '=====================================================';
END
    ELSE
    BEGIN
    PRINT '  - No hay usuarios que requieran migración manual';
END

    -- =====================================================
    -- PASO 5: Crear procedimiento para migración manual
    -- =====================================================
    
    PRINT 'Paso 5: Creando procedimiento para migración manual...';
    
    -- Crear procedimiento almacenado para migración manual de usuarios
    IF OBJECT_ID('dbo.MigrateUserRole') IS NOT NULL
        DROP PROCEDURE dbo.MigrateUserRole;
    
    EXEC('
    CREATE PROCEDURE [dbo].[MigrateUserRole]
        @UserId NVARCHAR(450),
        @NewRoleName NVARCHAR(256)
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @NewRoleId NVARCHAR(450);
        DECLARE @OldRoleName NVARCHAR(256);
        DECLARE @OldRoleId NVARCHAR(450);
        
        -- Verificar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = @UserId)
        BEGIN
            RAISERROR(''Usuario no encontrado'', 16, 1);
            RETURN;
        END
        
        -- Verificar que el nuevo rol existe
        SELECT @NewRoleId = Id FROM AspNetRoles WHERE Name = @NewRoleName AND IsActive = 1;
        IF @NewRoleId IS NULL
        BEGIN
            RAISERROR(''Rol no encontrado o inactivo'', 16, 1);
            RETURN;
        END
        
        -- Obtener rol actual
        SELECT @OldRoleId = r.Id, @OldRoleName = r.Name
        FROM AspNetUserRoles ur
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
        WHERE ur.UserId = @UserId AND ur.IsActive = 1;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            -- Eliminar rol actual
            IF @OldRoleId IS NOT NULL
            BEGIN
                DELETE FROM AspNetUserRoles 
                WHERE UserId = @UserId AND RoleId = @OldRoleId;
                
                PRINT ''Rol eliminado: '' + @OldRoleName;
            END
            
            -- Asignar nuevo rol
            INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt)
            VALUES (@UserId, @NewRoleId, 1, GETDATE());
            
            PRINT ''Usuario migrado exitosamente de '' + ISNULL(@OldRoleName, ''Sin rol'') + '' a '' + @NewRoleName;
            
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
    END
    ');
    
    PRINT '  - Procedimiento MigrateUserRole creado exitosamente';

    -- =====================================================
    -- PASO 6: Mostrar resumen final
    -- =====================================================
    
    PRINT 'Paso 6: Resumen final de migración...';
    
    -- Mostrar distribución actual de roles
    PRINT 'Distribución actual de usuarios por rol:';
    SELECT
    r.Name AS Rol,
    COUNT(ur.UserId) AS CantidadUsuarios,
    CASE 
            WHEN r.IsActive = 1 THEN 'Activo'
            ELSE 'Inactivo'
        END AS Estado
FROM AspNetRoles r
    LEFT JOIN AspNetUserRoles ur ON r.Id = ur.RoleId AND ur.IsActive = 1
GROUP BY r.Id, r.Name, r.IsActive
ORDER BY COUNT(ur.UserId) DESC;

    COMMIT TRANSACTION;
    
    PRINT '=====================================================';
    PRINT 'Migración de usuarios completada exitosamente!';
    PRINT '=====================================================';
    PRINT 'Usuarios migrados automáticamente: ' + CAST((@MonitorCount + @AgencyUserCount - @ManualMigrationCount) AS VARCHAR(10));
    PRINT 'Usuarios que requieren migración manual: ' + CAST(@ManualMigrationCount AS VARCHAR(10));
    PRINT '=====================================================';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    
    PRINT '=====================================================';
    PRINT 'ERROR en migración de usuarios:';
    PRINT 'Mensaje: ' + @ErrorMessage;
    PRINT 'Número: ' + CAST(@ErrorNumber AS VARCHAR(10));
    PRINT 'Línea: ' + CAST(@ErrorLine AS VARCHAR(10));
    PRINT '=====================================================';
    
    THROW;
END CATCH;

-- Limpiar tabla temporal
IF OBJECT_ID('tempdb..#UsersNeedingMigration') IS NOT NULL
    DROP TABLE #UsersNeedingMigration;

GO
