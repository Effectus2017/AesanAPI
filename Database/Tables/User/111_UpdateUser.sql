-- =============================================
-- Stored Procedure: 111_UpdateUser
-- Fecha: 2025-01-XX
-- Descripción: Actualiza un usuario y su asignación de agencia.
--              Reemplaza 110_UpdateUser con nueva lógica.
--              CRÍTICO: NO elimina todas las asignaciones (bug corregido).
--              El rol se obtiene mediante JOIN con AspNetUserRoles.
--              Calcula AgencyAssignmentType desde el rol obtenido.
-- =============================================

CREATE OR ALTER PROCEDURE [111_UpdateUser]
    @userId NVARCHAR(450),
    @email NVARCHAR(256),
    @emailConfirmed BIT,
    @isActive BIT,
    @isTemporalPasswordActived BIT,
    -- Datos de Staff (solo campos disponibles en UI)
    @firstName NVARCHAR(100),
    @middleName NVARCHAR(100),
    @fatherLastName NVARCHAR(100),
    @motherLastName NVARCHAR(100),
    @phoneNumber NVARCHAR(50),
    @agencyId INT,
    -- Rol
    @roleName NVARCHAR(256),
    -- Usuario que está realizando la asignación
    @assignedBy NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Logging para diagnóstico
    DECLARE @LogMessage NVARCHAR(MAX) = '';
    SET @LogMessage = 'Iniciando SP 111_UpdateUser - UserId: ' + @userId + ', Email: ' + @email;
    RAISERROR(@LogMessage, 10, 1) WITH NOWAIT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Actualizar datos de Identity User
        UPDATE AspNetUsers 
        SET 
            Email = @email,
            EmailConfirmed = @emailConfirmed,
            IsActive = @isActive,
            IsTemporalPasswordActived = @isTemporalPasswordActived,
            UpdatedAt = GETDATE()
        WHERE Id = @userId;
        
        SET @LogMessage = 'Paso 1 completado - AspNetUsers actualizado';
        RAISERROR(@LogMessage, 10, 1) WITH NOWAIT;

        -- 2. Actualizar datos de Staff (solo campos disponibles en UI)
        UPDATE Staff 
        SET 
            FirstName = @firstName,
            MiddleName = @middleName,
            FatherLastName = @fatherLastName,
            MotherLastName = @motherLastName,
            PhoneNumber = @phoneNumber,
            AgencyId = @agencyId,
            UpdatedAt = GETDATE()
        WHERE UserId = @userId;
        
        SET @LogMessage = 'Paso 2 completado - Staff actualizado';
        RAISERROR(@LogMessage, 10, 1) WITH NOWAIT;

        -- 3. Actualizar rol si es necesario
        IF @roleName IS NOT NULL
        BEGIN
            -- Obtener el ID del rol
            DECLARE @roleId NVARCHAR(450);
            SELECT @roleId = Id
            FROM AspNetRoles
            WHERE Name = @roleName;

            IF @roleId IS NOT NULL
            BEGIN
                -- Eliminar roles existentes
                DELETE FROM AspNetUserRoles WHERE UserId = @userId;

                -- Insertar nuevo rol
                INSERT INTO AspNetUserRoles
                    (UserId, RoleId, IsActive, CreatedAt)
                VALUES
                    (@userId, @roleId, 1, GETDATE());
            END
        END

        -- 4. Actualizar asignación de agencia (CRÍTICO: NO eliminar todas las asignaciones)
        IF @agencyId IS NOT NULL
        BEGIN
            -- Obtener el rol del usuario para calcular AgencyAssignmentType
            DECLARE @userRoleId NVARCHAR(450);
            DECLARE @userRoleName NVARCHAR(256);
            DECLARE @assignmentCategory VARCHAR(50);
            DECLARE @canBeOwner BIT = 0;
            DECLARE @agencyAssignmentType VARCHAR(50);
            
            SELECT TOP 1 
                @userRoleId = ur.RoleId,
                @userRoleName = r.Name
            FROM AspNetUserRoles ur
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @userId;
            
            IF @userRoleId IS NOT NULL
            BEGIN
                -- Obtener AssignmentCategory del rol
                SELECT 
                    @assignmentCategory = rac.AssignmentCategory,
                    @canBeOwner = rac.CanBeOwner
                FROM RoleAssignmentCategory rac
                WHERE rac.RoleId = @userRoleId;
                
                -- Calcular AgencyAssignmentType según AssignmentCategory
                IF @assignmentCategory = 'AGENCY'
                BEGIN
                    -- Para roles de agencia, verificar si puede ser owner
                    IF @canBeOwner = 1
                    BEGIN
                        -- Verificar si ya existe un AGENCY_OWNER activo para esta agencia
                        IF EXISTS (
                            SELECT 1
                            FROM AgencyUsers
                            WHERE AgencyId = @agencyId
                                AND AgencyAssignmentType = 'AGENCY_OWNER'
                                AND IsActive = 1
                                AND UserId != @userId
                        )
                        BEGIN
                            -- Ya existe un owner, asignar como STAFF
                            SET @agencyAssignmentType = 'AGENCY_STAFF';
                        END
                        ELSE
                        BEGIN
                            -- No hay owner, puede ser owner
                            SET @agencyAssignmentType = 'AGENCY_OWNER';
                        END
                    END
                    ELSE
                    BEGIN
                        SET @agencyAssignmentType = 'AGENCY_STAFF';
                    END
                END
                ELSE IF @assignmentCategory = 'NUTRE'
                BEGIN
                    -- Determinar tipo específico según el nombre del rol
                    IF @userRoleName LIKE '%Coordinador%' OR @userRoleName LIKE '%Coordinator%'
                        SET @agencyAssignmentType = 'NUTRE_COORDINATOR';
                    ELSE IF @userRoleName LIKE '%Evaluador%' OR @userRoleName LIKE '%Evaluator%'
                        SET @agencyAssignmentType = 'NUTRE_EVALUATOR';
                    ELSE IF @userRoleName LIKE '%Admin%' OR @userRoleName LIKE '%Administrador%'
                        SET @agencyAssignmentType = 'NUTRE_ADMIN';
                    ELSE IF @userRoleName LIKE '%Contaduría%' OR @userRoleName LIKE '%Accounting%' OR @userRoleName LIKE '%Contable%'
                        SET @agencyAssignmentType = 'NUTRE_ACCOUNTING';
                    ELSE
                        SET @agencyAssignmentType = 'NUTRE_EVALUATOR'; -- Default para roles NUTRE
                END
                ELSE
                BEGIN
                    -- Si no tiene AssignmentCategory, usar default
                    SET @agencyAssignmentType = 'AGENCY_STAFF';
                END
            END
            ELSE
            BEGIN
                -- Si no tiene rol, usar default
                SET @agencyAssignmentType = 'AGENCY_STAFF';
            END
            
            -- Actualizar o insertar asignación para esta agencia específica
            -- NO eliminar otras asignaciones del usuario
            IF EXISTS (
                SELECT 1
                FROM AgencyUsers
                WHERE UserId = @userId
                    AND AgencyId = @agencyId
                    AND IsActive = 1
            )
            BEGIN
                -- Actualizar asignación existente
                UPDATE AgencyUsers
                SET AgencyAssignmentType = @agencyAssignmentType,
                    UpdatedAt = GETUTCDATE(),
                    AssignedBy = @assignedBy
                WHERE UserId = @userId
                    AND AgencyId = @agencyId
                    AND IsActive = 1;
            END
            ELSE
            BEGIN
                -- Insertar nueva asignación
                INSERT INTO AgencyUsers
                    (UserId, AgencyId, AgencyAssignmentType, IsActive, AssignedBy, CreatedAt, AssignedDate)
                VALUES
                    (@userId, @agencyId, @agencyAssignmentType, 1, @assignedBy, GETUTCDATE(), GETUTCDATE());
            END
        END

        COMMIT TRANSACTION;

        -- Retornar éxito
        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Capturar información del error
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorLine INT = ERROR_LINE();
        
        SET @LogMessage = 'Error en SP 111_UpdateUser - Error: ' + @ErrorMessage + ', Line: ' + CAST(@ErrorLine AS NVARCHAR(10));
        RAISERROR(@LogMessage, 16, 1);
        
        -- Retornar error con información detallada
        SELECT
            0 AS Success,
            @ErrorMessage AS ErrorMessage,
            @ErrorNumber AS ErrorNumber,
            @ErrorLine AS ErrorLine;
    END CATCH
END;
GO
