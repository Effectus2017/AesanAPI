-- =====================================================
-- Script de Verificación Post-Migración - AESAN
-- Fecha: 2025-01-15
-- Descripción: Verifica que la migración se haya completado correctamente
-- =====================================================

PRINT '=====================================================';
PRINT 'VERIFICACIÓN POST-MIGRACIÓN AESAN';
PRINT '=====================================================';
PRINT 'Fecha: ' + CONVERT(VARCHAR(19), GETDATE(), 120);
PRINT '';

BEGIN TRY
    -- =====================================================
    -- VERIFICACIÓN 1: Roles AESAN
    -- =====================================================
    
    PRINT 'Verificación 1: Roles AESAN creados...';
    
    DECLARE @ExpectedRoles TABLE (RoleName NVARCHAR(256));
    INSERT INTO @ExpectedRoles
VALUES
    ('Super-Administrador'),
    ('Administrador'),
    ('Coordinador'),
    ('Evaluador'),
    ('Especialista'),
    ('Nutrición'),
    ('Contable'),
    ('Director Contable'),
    ('Abogado'),
    ('Sponsor Administrador'),
    ('Sponsor Director'),
    ('Sponsor Contable'),
    ('Funcionario Determinante'),
    ('Funcionario Confirmante'),
    ('Funcionario Revisión Independiente');
    
    DECLARE @MissingRoles INT;
    SELECT @MissingRoles = COUNT(*)
FROM @ExpectedRoles er
WHERE NOT EXISTS (SELECT 1
FROM AspNetRoles ar
WHERE ar.Name = er.RoleName AND ar.IsActive = 1);
    
    IF @MissingRoles = 0
        PRINT '  ✓ Todos los roles AESAN están presentes';
    ELSE
    BEGIN
    PRINT '  ✗ Faltan ' + CAST(@MissingRoles AS VARCHAR(10)) + ' roles AESAN';
    SELECT 'FALTANTE: ' + er.RoleName AS RolFaltante
    FROM @ExpectedRoles er
    WHERE NOT EXISTS (SELECT 1
    FROM AspNetRoles ar
    WHERE ar.Name = er.RoleName AND ar.IsActive = 1);
END
    
    -- =====================================================
    -- VERIFICACIÓN 2: Permisos Nuevos
    -- =====================================================
    
    PRINT '';
    PRINT 'Verificación 2: Permisos nuevos creados...';
    
    DECLARE @NewPermissionsCount INT;
    SELECT @NewPermissionsCount = COUNT(*)
FROM Permission
WHERE ValueKey LIKE 'evaluation.%'
    OR ValueKey LIKE 'intention.%'
    OR ValueKey LIKE 'visit.%'
    OR ValueKey LIKE 'eligibility.%'
    OR ValueKey LIKE 'contract.%'
    OR ValueKey LIKE 'accounting.%'
    OR ValueKey LIKE 'budget.%'
    OR ValueKey LIKE 'financial.%'
    OR ValueKey LIKE 'payroll.%'
    OR ValueKey LIKE 'sponsor.%'
    OR ValueKey LIKE 'form.%'
    OR ValueKey LIKE 'nutrition.%'
    OR ValueKey LIKE 'menu.%';
    
    IF @NewPermissionsCount > 0
        PRINT '  ✓ Se crearon ' + CAST(@NewPermissionsCount AS VARCHAR(10)) + ' permisos nuevos';
    ELSE
        PRINT '  ✗ No se encontraron permisos nuevos';
    
    -- =====================================================
    -- VERIFICACIÓN 3: Stored Procedures Actualizados
    -- =====================================================
    
    PRINT '';
    PRINT 'Verificación 3: Stored procedures actualizados...';
    
    -- Verificar que los SP críticos existen
    DECLARE @SPCount INT = 0;
    
    IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = '101_AssignAgencyToUser')
        SET @SPCount = @SPCount + 1;
    IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = '103_GetUserAssignedAgency')
        SET @SPCount = @SPCount + 1;
    IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = '100_GetAgencyProgramsByUserId')
        SET @SPCount = @SPCount + 1;
    IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = '100_GetEligibilityProgramsByUserId')
        SET @SPCount = @SPCount + 1;
    
    IF @SPCount = 4
        PRINT '  ✓ Todos los stored procedures críticos están actualizados';
    ELSE
        PRINT '  ✗ Faltan ' + CAST((4 - @SPCount) AS VARCHAR(10)) + ' stored procedures críticos';
    
    -- =====================================================
    -- VERIFICACIÓN 4: Roles Deprecados
    -- =====================================================
    
    PRINT '';
    PRINT 'Verificación 5: Roles deprecados manejados...';
    
    DECLARE @DeprecatedRolesCount INT;
    SELECT @DeprecatedRolesCount = COUNT(*)
FROM AspNetRoles
WHERE Name IN ('Monitor', 'Agency-User') AND IsActive = 1;
    
    IF @DeprecatedRolesCount = 0
        PRINT '  ✓ Roles deprecados (Monitor, Agency-User) están inactivos o eliminados';
    ELSE
    BEGIN
    PRINT '  ⚠ ADVERTENCIA: ' + CAST(@DeprecatedRolesCount AS VARCHAR(10)) + ' roles deprecados aún están activos';
    SELECT Name AS RolDeprecadoActivo
    FROM AspNetRoles
    WHERE Name IN ('Monitor', 'Agency-User') AND IsActive = 1;
END
    
    -- =====================================================
    -- VERIFICACIÓN 6: Usuarios con Roles Deprecados
    -- =====================================================
    
    PRINT '';
    PRINT 'Verificación 6: Usuarios con roles deprecados...';
    
    DECLARE @UsersWithDeprecatedRoles INT;
    SELECT @UsersWithDeprecatedRoles = COUNT(DISTINCT ur.UserId)
FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name IN ('Monitor', 'Agency-User') AND ur.IsActive = 1;
    
    IF @UsersWithDeprecatedRoles = 0
        PRINT '  ✓ No hay usuarios con roles deprecados';
    ELSE
    BEGIN
    PRINT '  ⚠ ADVERTENCIA: ' + CAST(@UsersWithDeprecatedRoles AS VARCHAR(10)) + ' usuarios aún tienen roles deprecados';
    PRINT '     Estos usuarios requieren migración manual';

    SELECT DISTINCT
        u.UserName,
        u.Email,
        r.Name AS RolDeprecado
    FROM AspNetUsers u
        INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE r.Name IN ('Monitor', 'Agency-User') AND ur.IsActive = 1;
END
    
    -- =====================================================
    -- RESUMEN FINAL
    -- =====================================================
    
    PRINT '';
    PRINT '=====================================================';
    PRINT 'RESUMEN DE VERIFICACIÓN';
    PRINT '=====================================================';
    PRINT 'Roles AESAN esperados: 15';
    PRINT 'Roles AESAN encontrados: ' + CAST((15 - @MissingRoles) AS VARCHAR(10));
    PRINT 'Permisos nuevos creados: ' + CAST(@NewPermissionsCount AS VARCHAR(10));
    PRINT 'Asignaciones de permisos: ' + CAST(@UserPermissionsCount AS VARCHAR(10));
    PRINT 'Stored procedures actualizados: ' + CAST(@SPCount AS VARCHAR(10)) + '/4';
    PRINT 'Usuarios con roles deprecados: ' + CAST(@UsersWithDeprecatedRoles AS VARCHAR(10));
    PRINT '=====================================================';
    
    IF @MissingRoles = 0 AND @NewPermissionsCount > 0 AND @UserPermissionsCount > 0 AND @SPCount = 4 AND @UsersWithDeprecatedRoles = 0
    BEGIN
    PRINT '🎉 MIGRACIÓN COMPLETADA EXITOSAMENTE';
    PRINT 'Todos los componentes están funcionando correctamente.';
END
    ELSE
    BEGIN
    PRINT '⚠️ MIGRACIÓN REQUIERE ATENCIÓN';
    PRINT 'Algunos componentes necesitan revisión manual.';
END
    
    PRINT '=====================================================';

END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    
    PRINT '=====================================================';
    PRINT 'ERROR en verificación:';
    PRINT 'Mensaje: ' + @ErrorMessage;
    PRINT 'Número: ' + CAST(@ErrorNumber AS VARCHAR(10));
    PRINT 'Línea: ' + CAST(@ErrorLine AS VARCHAR(10));
    PRINT '=====================================================';
    
    THROW;
END CATCH;

GO
