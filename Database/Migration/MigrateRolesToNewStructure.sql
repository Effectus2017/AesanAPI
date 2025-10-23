-- =====================================================
-- Script de Migración de Roles - Nueva Estructura AESAN
-- Fecha: 2025-01-15
-- Descripción: Migra los roles existentes a la nueva estructura requerida por AESAN
-- =====================================================

-- Verificar que las tablas existan
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'AspNetRoles')
BEGIN
    RAISERROR('La tabla AspNetRoles no existe. Ejecute primero el script de creación de tablas.', 16, 1);
    RETURN;
END

-- Verificar que la tabla Permission exista
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'Permission')
BEGIN
    RAISERROR('La tabla Permission no existe. Ejecute primero el script de creación de permisos.', 16, 1);
    RETURN;
END

PRINT 'Iniciando migración de roles a nueva estructura AESAN...';

BEGIN TRY
    BEGIN TRANSACTION;

    -- =====================================================
    -- PASO 1: Actualizar nombres de roles existentes
    -- =====================================================
    
    PRINT 'Paso 1: Actualizando nombres de roles existentes...';
    
    -- Actualizar SuperAdministrator
    IF EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'SuperAdministrator')
    BEGIN
    UPDATE AspNetRoles 
        SET Name = 'Super-Administrador', 
            NormalizedName = 'SUPER-ADMINISTRADOR',
            UpdatedAt = GETDATE()
        WHERE Name = 'SuperAdministrator';
    PRINT '  - SuperAdministrator actualizado a Super-Administrador';
END

    -- Actualizar Administrator
    IF EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Administrator')
    BEGIN
    UPDATE AspNetRoles 
        SET Name = 'Administrador', 
            NormalizedName = 'ADMINISTRADOR',
            UpdatedAt = GETDATE()
        WHERE Name = 'Administrator';
    PRINT '  - Administrator actualizado a Administrador';
END

    -- Actualizar Agency-Administrator
    IF EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Agency-Administrator')
    BEGIN
    UPDATE AspNetRoles 
        SET Name = 'Sponsor-Administrador', 
            NormalizedName = 'SPONSOR-ADMINISTRADOR',
            UpdatedAt = GETDATE()
        WHERE Name = 'Agency-Administrator';
    PRINT '  - Agency-Administrator actualizado a Sponsor-Administrador';
END

    -- =====================================================
    -- PASO 2: Crear nuevos roles internos AESAN
    -- =====================================================
    
    PRINT 'Paso 2: Creando nuevos roles internos AESAN...';
    
    -- Coordinador
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Coordinador')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Coordinador', 'COORDINADOR', NEWID(), 1, GETDATE());
    PRINT '  - Coordinador creado';
END

    -- Evaluador
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Evaluador')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Evaluador', 'EVALUADOR', NEWID(), 1, GETDATE());
    PRINT '  - Evaluador creado';
END

    -- Especialista
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Especialista')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Especialista', 'ESPECIALISTA', NEWID(), 1, GETDATE());
    PRINT '  - Especialista creado';
END

    -- Nutrición
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Nutrición')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Nutrición', 'NUTRICIÓN', NEWID(), 1, GETDATE());
    PRINT '  - Nutrición creado';
END

    -- Contable
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Contable')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Contable', 'CONTABLE', NEWID(), 1, GETDATE());
    PRINT '  - Contable creado';
END

    -- Director Contable
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Director Contable')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Director Contable', 'DIRECTOR CONTABLE', NEWID(), 1, GETDATE());
    PRINT '  - Director Contable creado';
END

    -- Abogado
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Abogado')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Abogado', 'ABOGADO', NEWID(), 1, GETDATE());
    PRINT '  - Abogado creado';
END

    -- =====================================================
    -- PASO 3: Crear nuevos roles de Sponsor
    -- =====================================================
    
    PRINT 'Paso 3: Creando nuevos roles de Sponsor...';
    
    -- Sponsor Director
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Sponsor Director')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Sponsor Director', 'SPONSOR DIRECTOR', NEWID(), 1, GETDATE());
    PRINT '  - Sponsor Director creado';
END

    -- Sponsor Contable
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Sponsor Contable')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Sponsor Contable', 'SPONSOR CONTABLE', NEWID(), 1, GETDATE());
    PRINT '  - Sponsor Contable creado';
END

    -- =====================================================
    -- PASO 4: Crear nuevos roles de Funcionarios
    -- =====================================================
    
    PRINT 'Paso 4: Creando nuevos roles de Funcionarios...';
    
    -- Funcionario Determinante
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Funcionario Determinante')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Funcionario Determinante', 'FUNCIONARIO DETERMINANTE', NEWID(), 1, GETDATE());
    PRINT '  - Funcionario Determinante creado';
END

    -- Funcionario Confirmante
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Funcionario Confirmante')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Funcionario Confirmante', 'FUNCIONARIO CONFIRMANTE', NEWID(), 1, GETDATE());
    PRINT '  - Funcionario Confirmante creado';
END

    -- Funcionario Revisión Independiente
    IF NOT EXISTS (SELECT 1
FROM AspNetRoles
WHERE Name = 'Funcionario Revisión Independiente')
    BEGIN
    INSERT INTO AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
    VALUES
        (NEWID(), 'Funcionario Revisión Independiente', 'FUNCIONARIO REVISIÓN INDEPENDIENTE', NEWID(), 1, GETDATE());
    PRINT '  - Funcionario Revisión Independiente creado';
END

    -- =====================================================
    -- PASO 5: Manejar rol Monitor (deprecado)
    -- =====================================================
    
    PRINT 'Paso 5: Verificando usuarios con rol Monitor...';
    
    DECLARE @MonitorUsersCount INT;
    SELECT @MonitorUsersCount = COUNT(*)
FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Monitor' AND ur.IsActive = 1;

    IF @MonitorUsersCount > 0
    BEGIN
    PRINT '  - ADVERTENCIA: Existen ' + CAST(@MonitorUsersCount AS VARCHAR(10)) + ' usuarios con rol Monitor';
    PRINT '  - Estos usuarios necesitan ser migrados manualmente a roles apropiados';
    PRINT '  - El rol Monitor será marcado como inactivo pero no eliminado';

    -- Marcar rol Monitor como inactivo
    UPDATE AspNetRoles 
        SET IsActive = 0, UpdatedAt = GETDATE()
        WHERE Name = 'Monitor';
    PRINT '  - Rol Monitor marcado como inactivo';
END
    ELSE
    BEGIN
    PRINT '  - No hay usuarios con rol Monitor, se puede eliminar';
    -- Eliminar rol Monitor si no hay usuarios
    DELETE FROM AspNetRoles WHERE Name = 'Monitor';
    PRINT '  - Rol Monitor eliminado';
END

    -- =====================================================
    -- PASO 6: Manejar rol Agency-User (deprecado)
    -- =====================================================
    
    PRINT 'Paso 6: Verificando usuarios con rol Agency-User...';
    
    DECLARE @AgencyUserCount INT;
    SELECT @AgencyUserCount = COUNT(*)
FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Agency-User' AND ur.IsActive = 1;

    IF @AgencyUserCount > 0
    BEGIN
    PRINT '  - ADVERTENCIA: Existen ' + CAST(@AgencyUserCount AS VARCHAR(10)) + ' usuarios con rol Agency-User';
    PRINT '  - Estos usuarios necesitan ser migrados a roles de funcionario apropiados';
    PRINT '  - El rol Agency-User será marcado como inactivo pero no eliminado';

    -- Marcar rol Agency-User como inactivo
    UPDATE AspNetRoles 
        SET IsActive = 0, UpdatedAt = GETDATE()
        WHERE Name = 'Agency-User';
    PRINT '  - Rol Agency-User marcado como inactivo';
END
    ELSE
    BEGIN
    PRINT '  - No hay usuarios con rol Agency-User, se puede eliminar';
    -- Eliminar rol Agency-User si no hay usuarios
    DELETE FROM AspNetRoles WHERE Name = 'Agency-User';
    PRINT '  - Rol Agency-User eliminado';
END

    COMMIT TRANSACTION;
    
    PRINT '=====================================================';
    PRINT 'Migración de roles completada exitosamente!';
    PRINT '=====================================================';
    
    -- Mostrar resumen de roles actuales
    PRINT 'Roles actuales en el sistema:';
    SELECT
    Id,
    Name,
    NormalizedName,
    IsActive,
    CreatedAt
FROM AspNetRoles
ORDER BY Name;

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    
    PRINT '=====================================================';
    PRINT 'ERROR en migración de roles:';
    PRINT 'Mensaje: ' + @ErrorMessage;
    PRINT 'Número: ' + CAST(@ErrorNumber AS VARCHAR(10));
    PRINT 'Línea: ' + CAST(@ErrorLine AS VARCHAR(10));
    PRINT '=====================================================';
    
    THROW;
END CATCH;

GO
