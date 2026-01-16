-- =============================================
-- Script: Migrar datos existentes a AgencyAssignmentType
-- Fecha: 2025-01-XX
-- Descripción: Migra los datos existentes de IsOwner/IsMonitor a AgencyAssignmentType.
--              El rol se obtiene mediante JOIN con AspNetUserRoles.
-- =============================================

PRINT 'Iniciando migración de datos existentes...';
PRINT '';

BEGIN TRY
    BEGIN TRANSACTION;

    -- =============================================
    -- PASO 1: Migrar IsOwner = 1 → AGENCY_OWNER
    -- =============================================
    PRINT 'Paso 1: Migrando IsOwner = 1 → AGENCY_OWNER...';
    
    UPDATE au
    SET AgencyAssignmentType = 'AGENCY_OWNER',
        UpdatedAt = GETUTCDATE()
    FROM AgencyUsers au
    WHERE au.IsOwner = 1
        AND au.IsActive = 1
        AND (au.AgencyAssignmentType IS NULL OR au.AgencyAssignmentType != 'AGENCY_OWNER');
    
    DECLARE @rowsUpdatedOwner INT = @@ROWCOUNT;
    PRINT '  - ' + CAST(@rowsUpdatedOwner AS VARCHAR(10)) + ' registros actualizados a AGENCY_OWNER.';
    
    -- =============================================
    -- PASO 2: Migrar IsMonitor = 1 → AgencyAssignmentType según rol
    -- =============================================
    PRINT '';
    PRINT 'Paso 2: Migrando IsMonitor = 1 → AgencyAssignmentType según rol...';
    
    -- Para usuarios con rol Monitor → NUTRE_EVALUATOR
    UPDATE au
    SET AgencyAssignmentType = 'NUTRE_EVALUATOR',
        UpdatedAt = GETUTCDATE()
    FROM AgencyUsers au
    INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE au.IsMonitor = 1
        AND au.IsActive = 1
        AND r.Name = 'Monitor'
        AND (au.AgencyAssignmentType IS NULL OR au.AgencyAssignmentType != 'NUTRE_EVALUATOR');
    
    DECLARE @rowsUpdatedMonitor INT = @@ROWCOUNT;
    PRINT '  - ' + CAST(@rowsUpdatedMonitor AS VARCHAR(10)) + ' registros de Monitor actualizados a NUTRE_EVALUATOR.';
    
    -- Para usuarios con rol Coordinador → NUTRE_COORDINATOR
    UPDATE au
    SET AgencyAssignmentType = 'NUTRE_COORDINATOR',
        UpdatedAt = GETUTCDATE()
    FROM AgencyUsers au
    INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE au.IsMonitor = 1
        AND au.IsActive = 1
        AND (r.Name = 'Coordinador' OR r.Name LIKE '%Coordinador%')
        AND (au.AgencyAssignmentType IS NULL OR au.AgencyAssignmentType != 'NUTRE_COORDINATOR');
    
    DECLARE @rowsUpdatedCoordinator INT = @@ROWCOUNT;
    PRINT '  - ' + CAST(@rowsUpdatedCoordinator AS VARCHAR(10)) + ' registros de Coordinador actualizados a NUTRE_COORDINATOR.';
    
    -- Para usuarios con rol Evaluador → NUTRE_EVALUATOR
    UPDATE au
    SET AgencyAssignmentType = 'NUTRE_EVALUATOR',
        UpdatedAt = GETUTCDATE()
    FROM AgencyUsers au
    INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE au.IsMonitor = 1
        AND au.IsActive = 1
        AND (r.Name = 'Evaluador' OR r.Name LIKE '%Evaluador%')
        AND (au.AgencyAssignmentType IS NULL OR au.AgencyAssignmentType != 'NUTRE_EVALUATOR');
    
    DECLARE @rowsUpdatedEvaluator INT = @@ROWCOUNT;
    PRINT '  - ' + CAST(@rowsUpdatedEvaluator AS VARCHAR(10)) + ' registros de Evaluador actualizados a NUTRE_EVALUATOR.';
    
    -- Para otros roles NUTRE con IsMonitor = 1 → NUTRE_EVALUATOR (default)
    UPDATE au
    SET AgencyAssignmentType = 'NUTRE_EVALUATOR',
        UpdatedAt = GETUTCDATE()
    FROM AgencyUsers au
    INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    INNER JOIN RoleAssignmentCategory rac ON r.Id = rac.RoleId
    WHERE au.IsMonitor = 1
        AND au.IsActive = 1
        AND rac.AssignmentCategory = 'NUTRE'
        AND (au.AgencyAssignmentType IS NULL OR au.AgencyAssignmentType NOT LIKE 'NUTRE_%');
    
    DECLARE @rowsUpdatedOtherNutre INT = @@ROWCOUNT;
    PRINT '  - ' + CAST(@rowsUpdatedOtherNutre AS VARCHAR(10)) + ' registros de otros roles NUTRE actualizados a NUTRE_EVALUATOR.';
    
    -- =============================================
    -- PASO 3: Migrar IsOwner = 0 AND IsMonitor = 0 → AGENCY_STAFF
    -- =============================================
    PRINT '';
    PRINT 'Paso 3: Migrando IsOwner = 0 AND IsMonitor = 0 → AGENCY_STAFF...';
    
    UPDATE au
    SET AgencyAssignmentType = 'AGENCY_STAFF',
        UpdatedAt = GETUTCDATE()
    FROM AgencyUsers au
    WHERE au.IsOwner = 0
        AND au.IsMonitor = 0
        AND au.IsActive = 1
        AND (au.AgencyAssignmentType IS NULL OR au.AgencyAssignmentType != 'AGENCY_STAFF');
    
    DECLARE @rowsUpdatedStaff INT = @@ROWCOUNT;
    PRINT '  - ' + CAST(@rowsUpdatedStaff AS VARCHAR(10)) + ' registros actualizados a AGENCY_STAFF.';
    
    -- =============================================
    -- PASO 4: Validar que todos los registros activos tengan AgencyAssignmentType
    -- =============================================
    PRINT '';
    PRINT 'Paso 4: Validando migración...';
    
    DECLARE @registrosSinMigrar INT;
    SELECT @registrosSinMigrar = COUNT(*)
    FROM AgencyUsers
    WHERE IsActive = 1 
        AND AgencyAssignmentType IS NULL;
    
    IF @registrosSinMigrar > 0
    BEGIN
        PRINT '  ⚠️ ADVERTENCIA: ' + CAST(@registrosSinMigrar AS VARCHAR(10)) + ' registros activos sin AgencyAssignmentType.';
        PRINT '  Mostrando registros sin migrar:';
        
        SELECT 
            au.Id,
            au.UserId,
            au.AgencyId,
            au.IsOwner,
            au.IsMonitor,
            au.IsActive,
            r.Name AS RoleName
        FROM AgencyUsers au
        LEFT JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        WHERE au.IsActive = 1 
            AND au.AgencyAssignmentType IS NULL;
        
        -- Intentar asignar un valor por defecto según el rol
        PRINT '';
        PRINT '  Intentando asignar valores por defecto...';
        
        -- Para roles AGENCY sin IsOwner → AGENCY_STAFF
        UPDATE au
        SET AgencyAssignmentType = 'AGENCY_STAFF',
            UpdatedAt = GETUTCDATE()
        FROM AgencyUsers au
        INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
        INNER JOIN RoleAssignmentCategory rac ON r.Id = rac.RoleId
        WHERE au.IsActive = 1 
            AND au.AgencyAssignmentType IS NULL
            AND rac.AssignmentCategory = 'AGENCY'
            AND au.IsOwner = 0;
        
        -- Para roles NUTRE sin IsMonitor → NUTRE_EVALUATOR (default)
        UPDATE au
        SET AgencyAssignmentType = 'NUTRE_EVALUATOR',
            UpdatedAt = GETUTCDATE()
        FROM AgencyUsers au
        INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
        INNER JOIN RoleAssignmentCategory rac ON r.Id = rac.RoleId
        WHERE au.IsActive = 1 
            AND au.AgencyAssignmentType IS NULL
            AND rac.AssignmentCategory = 'NUTRE'
            AND au.IsMonitor = 0;
        
        -- Verificar nuevamente
        SELECT @registrosSinMigrar = COUNT(*)
        FROM AgencyUsers
        WHERE IsActive = 1 
            AND AgencyAssignmentType IS NULL;
        
        IF @registrosSinMigrar > 0
        BEGIN
            PRINT '  ⚠️ ADVERTENCIA: Aún quedan ' + CAST(@registrosSinMigrar AS VARCHAR(10)) + ' registros sin migrar.';
            PRINT '  Estos registros pueden requerir intervención manual.';
        END
        ELSE
        BEGIN
            PRINT '  ✓ Todos los registros han sido migrados exitosamente.';
        END
    END
    ELSE
    BEGIN
        PRINT '  ✓ Todos los registros activos tienen AgencyAssignmentType.';
    END
    
    -- =============================================
    -- PASO 5: Verificar que todos los usuarios tienen rol
    -- =============================================
    PRINT '';
    PRINT 'Paso 5: Verificando que todos los usuarios tienen rol...';
    
    DECLARE @usuariosSinRol INT;
    SELECT @usuariosSinRol = COUNT(DISTINCT au.UserId)
    FROM AgencyUsers au
    LEFT JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
    WHERE au.IsActive = 1 
        AND ur.RoleId IS NULL;
    
    IF @usuariosSinRol > 0
    BEGIN
        PRINT '  ⚠️ ADVERTENCIA: ' + CAST(@usuariosSinRol AS VARCHAR(10)) + ' usuarios activos sin rol asignado.';
        PRINT '  Mostrando usuarios sin rol:';
        
        SELECT DISTINCT
            au.UserId,
            COUNT(*) AS NumeroAgencias
        FROM AgencyUsers au
        LEFT JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
        WHERE au.IsActive = 1 
            AND ur.RoleId IS NULL
        GROUP BY au.UserId;
        
        PRINT '  Estos usuarios pueden requerir intervención manual.';
    END
    ELSE
    BEGIN
        PRINT '  ✓ Todos los usuarios activos tienen rol asignado.';
    END
    
    COMMIT TRANSACTION;
    
    PRINT '';
    PRINT '=============================================';
    PRINT 'Migración completada exitosamente.';
    PRINT '=============================================';
    PRINT '';
    PRINT 'Resumen:';
    PRINT '  - AGENCY_OWNER: ' + CAST(@rowsUpdatedOwner AS VARCHAR(10)) + ' registros';
    PRINT '  - NUTRE_EVALUATOR (Monitor): ' + CAST(@rowsUpdatedMonitor AS VARCHAR(10)) + ' registros';
    PRINT '  - NUTRE_COORDINATOR: ' + CAST(@rowsUpdatedCoordinator AS VARCHAR(10)) + ' registros';
    PRINT '  - NUTRE_EVALUATOR (Evaluador): ' + CAST(@rowsUpdatedEvaluator AS VARCHAR(10)) + ' registros';
    PRINT '  - NUTRE_EVALUATOR (Otros NUTRE): ' + CAST(@rowsUpdatedOtherNutre AS VARCHAR(10)) + ' registros';
    PRINT '  - AGENCY_STAFF: ' + CAST(@rowsUpdatedStaff AS VARCHAR(10)) + ' registros';
    PRINT '';
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT '';
    PRINT '=============================================';
    PRINT 'ERROR en la migración:';
    PRINT @ErrorMessage;
    PRINT '=============================================';
    
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH
GO
