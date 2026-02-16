-- =============================================
-- Migración: Nuevos roles AESAN (Rol en NUTRE)
-- Descripción: (1) Renombrar SuperAdministrator a Super-Administrator (y Super-Administrador si existe).
--              (2) Crear los 10 roles NUTRE que falten. (3) Marcar como inactivos Program-Coordinator, Director, Accounting (no se eliminan).
--              (4) IsAesanRole = 1 para todos excepto Agency-Administrator y Agency-User (solo esos dos no son AESAN).
--              No se quitan: Super-Administrator, Administrator, Agency-Administrator, Agency-User.
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    RAISERROR('La tabla AspNetRoles no existe.', 16, 1);
    RETURN;
END

BEGIN TRY
    BEGIN TRANSACTION;

    -- =====================================================
    -- PASO 1: Renombrar SuperAdministrator a Super-Administrator
    -- =====================================================
    IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdministrator')
    BEGIN
        UPDATE AspNetRoles
        SET Name = 'Super-Administrator',
            NormalizedName = 'SUPER-ADMINISTRATOR',
            UpdatedAt = GETDATE()
        WHERE Name = 'SuperAdministrator';
        PRINT 'SuperAdministrator renombrado a Super-Administrator.';
    END

    IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Super-Administrador')
    BEGIN
        UPDATE AspNetRoles
        SET Name = 'Super-Administrator',
            NormalizedName = 'SUPER-ADMINISTRATOR',
            UpdatedAt = GETDATE()
        WHERE Name = 'Super-Administrador';
        PRINT 'Super-Administrador renombrado a Super-Administrator.';
    END

    -- =====================================================
    -- PASO 2: Insertar los 10 roles NUTRE (si no existen)
    -- =====================================================
    DECLARE @roles TABLE (Name NVARCHAR(256), NormalizedName NVARCHAR(256));
    INSERT INTO @roles (Name, NormalizedName) VALUES
        (N'Finanzas', N'FINANZAS'),
        (N'Contable', N'CONTABLE'),
        (N'Coordinadora de Monitoría', N'COORDINADORA DE MONITORÍA'),
        (N'Oficial de Cumplimiento', N'OFICIAL DE CUMPLIMIENTO'),
        (N'Especialista', N'ESPECIALISTA'),
        (N'Analista', N'ANALISTA'),
        (N'Coordinadora', N'COORDINADORA'),
        (N'Evaluadora', N'EVALUADORA'),
        (N'Nutricionista', N'NUTRICIONISTA'),
        (N'Asesor Legal', N'ASESOR LEGAL');

    DECLARE @name NVARCHAR(256), @norm NVARCHAR(256);
    DECLARE rc CURSOR LOCAL FAST_FORWARD FOR SELECT Name, NormalizedName FROM @roles;
    OPEN rc;
    FETCH NEXT FROM rc INTO @name, @norm;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = @name)
        BEGIN
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp, IsActive, CreatedAt)
            VALUES (NEWID(), @name, @norm, NEWID(), 1, GETDATE());
            PRINT 'Rol creado: ' + @name;
        END
        FETCH NEXT FROM rc INTO @name, @norm;
    END
    CLOSE rc;
    DEALLOCATE rc;

    -- =====================================================
    -- PASO 3: Marcar como inactivos Program-Coordinator, Director, Accounting
    -- =====================================================
    UPDATE AspNetRoles
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Name IN ('Program-Coordinator', 'Director', 'Accounting')
      AND IsActive = 1;
    IF @@ROWCOUNT > 0
        PRINT 'Roles Program-Coordinator, Director y/o Accounting marcados como inactivos.';

    -- =====================================================
    -- PASO 4: RoleAssignmentCategory para los nuevos roles NUTRE (si existe la tabla)
    -- =====================================================
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RoleAssignmentCategory')
    BEGIN
        MERGE RoleAssignmentCategory AS target
        USING (
            SELECT Id FROM AspNetRoles
            WHERE Name IN (
                N'Finanzas', N'Contable', N'Coordinadora de Monitoría', N'Oficial de Cumplimiento',
                N'Especialista', N'Analista', N'Coordinadora', N'Evaluadora', N'Nutricionista', N'Asesor Legal'
            ) AND IsActive = 1
        ) AS source ON target.RoleId = source.Id
        WHEN NOT MATCHED THEN
            INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
            VALUES (source.Id, 'NUTRE', 0, GETUTCDATE());
        PRINT 'RoleAssignmentCategory actualizado para nuevos roles NUTRE.';
    END

    -- =====================================================
    -- PASO 5: IsAesanRole = 1 para todos excepto roles de agencia
    -- Todos son AESAN roles excepto Agency-Administrator y Agency-User.
    -- =====================================================
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetRoles') AND name = 'IsAesanRole')
    BEGIN
        UPDATE AspNetRoles
        SET IsAesanRole = 1,
            UpdatedAt = GETDATE()
        WHERE Name NOT IN ('Agency-Administrator', 'Agency-User');
        PRINT 'IsAesanRole = 1 actualizado para todos los roles excepto Agency-Administrator y Agency-User.';
    END

    COMMIT TRANSACTION;
    PRINT 'Migración Mig_AesanRolesNuevos completada.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT 'Error: ' + @Msg;
    THROW;
END CATCH;
GO
