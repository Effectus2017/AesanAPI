-- =====================================================
-- Script de Corrección de Stored Procedures - AESAN
-- Fecha: 2025-01-15
-- Descripción: Actualiza stored procedures que referencian roles deprecados
-- =====================================================

PRINT 'Iniciando corrección de stored procedures con roles deprecados...';

BEGIN TRY
    BEGIN TRANSACTION;

    -- =====================================================
    -- CORRECCIÓN 1: 101_AssignAgencyToUser.sql
    -- =====================================================
    
    PRINT 'Corrigiendo 101_AssignAgencyToUser.sql...';
    
    -- Actualizar validación de roles para owners
    IF EXISTS (SELECT 1
FROM sys.procedures
WHERE name = '101_AssignAgencyToUser')
    BEGIN
    EXEC('
        CREATE OR ALTER PROCEDURE [dbo].[101_AssignAgencyToUser]
            @userId NVARCHAR(450),
            @agencyId INT,
            @assignedBy NVARCHAR(450),
            @isOwner BIT
        AS
        BEGIN
            SET NOCOUNT ON;

            DECLARE @Id INT;
            DECLARE @userRole NVARCHAR(50);

            -- Obtener el rol del usuario
            SELECT TOP 1
                @userRole = r.Name
            FROM AspNetUserRoles ur
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @userId;

            -- ✅ CORREGIDO: Validar que solo roles de Sponsor puedan ser owners
            IF @isOwner = 1 AND @userRole NOT IN (''Sponsor Administrador'', ''Sponsor Director'', ''Sponsor Contable'')
            BEGIN
                RAISERROR (''Solo un Sponsor Administrador, Sponsor Director o Sponsor Contable puede ser propietario de una agencia.'', 16, 1);
                RETURN -1;
            END

            -- Si ya existe una asignación activa para este usuario y agencia, actualizarla
            IF EXISTS (
                SELECT 1
            FROM AgencyUsers
            WHERE UserId = @userId
                AND AgencyId = @agencyId
                AND IsActive = 1
            )
            BEGIN
                UPDATE AgencyUsers
                SET IsOwner = @isOwner,
                    UpdatedAt = GETUTCDATE(),
                    AssignedBy = @assignedBy
                WHERE UserId = @userId
                    AND AgencyId = @agencyId
                    AND IsActive = 1;

                SELECT @Id = Id
                FROM AgencyUsers
                WHERE UserId = @userId
                    AND AgencyId = @agencyId
                    AND IsActive = 1;
            END
            ELSE
            BEGIN
                -- Insertar la nueva asignación
                INSERT INTO AgencyUsers
                    (UserId, AgencyId, IsOwner, IsMonitor, IsActive, AssignedBy, CreatedAt)
                VALUES
                    (@userId, @agencyId, @isOwner, 0, 1, @assignedBy, GETUTCDATE());

                SET @Id = SCOPE_IDENTITY();
            END

            -- Retornar el ID de la asignación
            SELECT @Id AS Id;
        END
        ');

    PRINT '  ✓ 101_AssignAgencyToUser.sql actualizado';
END

    -- =====================================================
    -- CORRECCIÓN 2: 103_GetUserAssignedAgency.sql
    -- =====================================================
    
    PRINT 'Corrigiendo 103_GetUserAssignedAgency.sql...';
    
    IF EXISTS (SELECT 1
FROM sys.procedures
WHERE name = '103_GetUserAssignedAgency')
    BEGIN
    EXEC('
        CREATE OR ALTER PROCEDURE [dbo].[103_GetUserAssignedAgency]
            @userId NVARCHAR(450)
        AS
        BEGIN
            SET NOCOUNT ON;

            -- ✅ CORREGIDO: Verificar si el usuario es Sponsor Administrador
            DECLARE @isSponsorAdmin BIT = 0;
            SELECT @isSponsorAdmin = 1
            FROM AspNetUserRoles ur
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @userId AND r.Name = ''Sponsor Administrador'';

            IF @isSponsorAdmin = 1
            BEGIN
                -- Para Sponsor Administrador, obtener la agencia donde es owner
                SELECT TOP 1
                    a.Id,
                    a.Name,
                    a.Address,
                    a.Phone,
                    a.Email,
                    a.IsActive,
                    a.CreatedAt,
                    a.UpdatedAt,
                    au.IsOwner,
                    au.IsMonitor
                FROM [dbo].[Agency] a
                    INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
                WHERE au.UserId = @userId
                    AND au.IsOwner = 1
                    AND au.IsActive = 1
                ORDER BY au.CreatedAt DESC;
            END
            ELSE
            BEGIN
                -- Para otros usuarios, obtener la agencia principal (donde no es monitor)
                SELECT TOP 1
                    a.Id,
                    a.Name,
                    a.Address,
                    a.Phone,
                    a.Email,
                    a.IsActive,
                    a.CreatedAt,
                    a.UpdatedAt,
                    au.IsOwner,
                    au.IsMonitor
                FROM [dbo].[Agency] a
                    INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
                WHERE au.UserId = @userId
                    AND au.IsMonitor = 0
                    AND au.IsActive = 1
                ORDER BY au.CreatedAt DESC;
            END
        END
        ');

    PRINT '  ✓ 103_GetUserAssignedAgency.sql actualizado';
END

    -- =====================================================
    -- CORRECCIÓN 3: 100_GetAgencyProgramsByUserId.sql
    -- =====================================================
    
    PRINT 'Corrigiendo 100_GetAgencyProgramsByUserId.sql...';
    
    IF EXISTS (SELECT 1
FROM sys.procedures
WHERE name = '100_GetAgencyProgramsByUserId')
    BEGIN
    EXEC('
        CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyProgramsByUserId]
            @userId NVARCHAR(450)
        AS
        BEGIN
            SET NOCOUNT ON;

            -- ✅ CORREGIDO: Obtener programas para usuarios con roles de Sponsor
            -- (reemplaza la lógica específica de Monitor)
            SELECT DISTINCT
                p.*
            FROM AspNetUsers u
                JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                JOIN AspNetRoles r ON ur.RoleId = r.Id
                JOIN UserProgram up ON u.Id = up.UserId
                JOIN Program p ON up.ProgramId = p.Id
                JOIN AgencyProgram ap ON p.Id = ap.ProgramId
                JOIN Agency a ON ap.AgencyId = a.Id
                JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
            WHERE 
                r.Name IN (''Sponsor Administrador'', ''Sponsor Director'', ''Sponsor Contable'')
                AND u.Id = @userId
                AND a.IsActive = 1;
        END
        ');

    PRINT '  ✓ 100_GetAgencyProgramsByUserId.sql actualizado';
END

    -- =====================================================
    -- CORRECCIÓN 4: Crear nuevo SP para roles de Funcionarios
    -- =====================================================
    
    PRINT 'Creando nuevo stored procedure para roles de Funcionarios...';
    
    EXEC('
    CREATE OR ALTER PROCEDURE [dbo].[100_GetEligibilityProgramsByUserId]
        @userId NVARCHAR(450)
    AS
    BEGIN
        SET NOCOUNT ON;

        -- Obtener programas para usuarios con roles de Funcionarios (elegibilidad)
        SELECT DISTINCT
            p.*
        FROM AspNetUsers u
            JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            JOIN AspNetRoles r ON ur.RoleId = r.Id
            JOIN UserProgram up ON u.Id = up.UserId
            JOIN Program p ON up.ProgramId = p.Id
        WHERE 
            r.Name IN (''Funcionario Determinante'', ''Funcionario Confirmante'', ''Funcionario Revisión Independiente'')
            AND u.Id = @userId
            AND p.IsActive = 1;
    END
    ');
    
    PRINT '  ✓ 100_GetEligibilityProgramsByUserId.sql creado';

    COMMIT TRANSACTION;
    
    PRINT '=====================================================';
    PRINT 'Corrección de stored procedures completada exitosamente!';
    PRINT '=====================================================';
    PRINT 'Stored procedures actualizados:';
    PRINT '  - 101_AssignAgencyToUser.sql (eliminado @isMonitor, solo roles de Sponsor)';
    PRINT '  - 103_GetUserAssignedAgency.sql (Sponsor Administrador)';
    PRINT '  - 100_GetAgencyProgramsByUserId.sql (roles de Sponsor)';
    PRINT '  - 100_GetEligibilityProgramsByUserId.sql (roles de Funcionarios)';
    PRINT '=====================================================';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    
    PRINT '=====================================================';
    PRINT 'ERROR en corrección de stored procedures:';
    PRINT 'Mensaje: ' + @ErrorMessage;
    PRINT 'Número: ' + CAST(@ErrorNumber AS VARCHAR(10));
    PRINT 'Línea: ' + CAST(@ErrorLine AS VARCHAR(10));
    PRINT '=====================================================';
    
    THROW;
END CATCH;

GO
