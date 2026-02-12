-- =============================================
-- Stored Procedure: 112_UpdateUser
-- Fecha: 2025-02-XX
-- Descripción: Actualiza un usuario y su asignación de agencia.
--              Reemplaza 111_UpdateUser con soporte para múltiples roles.
--              @roleNames: lista de nombres de rol separados por coma (ej: 'Administrator,Coordinadora de Monitoría').
--              Para AgencyAssignmentType se usa el primer rol de la lista.
-- =============================================

CREATE OR ALTER PROCEDURE [112_UpdateUser]
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
    -- Roles (lista separada por comas, ej: 'Administrator,Coordinadora de Monitoría')
    @roleNames NVARCHAR(MAX),
    -- Programa asignado al usuario (opcional; para filtrado de información)
    @programId INT = NULL,
    -- Usuario que está realizando la asignación
    @assignedBy NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Logging para diagnóstico
    DECLARE @LogMessage NVARCHAR(MAX) = '';
    SET @LogMessage = 'Iniciando SP 112_UpdateUser - UserId: ' + @userId + ', Email: ' + @email;
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

        -- 2. Actualizar o insertar datos de Staff (upsert: si no existe Staff, crear uno)
        IF EXISTS (SELECT 1 FROM Staff WHERE UserId = @userId)
        BEGIN
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
        END
        ELSE
        BEGIN
            -- Usar IDs válidos que existan en OptionSelection/City/Region (mismo patrón que 102_SyncUsersToStaff)
            INSERT INTO Staff
                (FirstName, MiddleName, FatherLastName, MotherLastName, StatusId, PositionId, StaffTypeId,
                 StaffClassificationId, BirthDate, Email, PhoneNumber, PostalAddress, CityId, RegionId, ZipCode,
                 AgencyId, UserId, CreatedAt, IsActive)
            VALUES
                (@firstName, @middleName, @fatherLastName, @motherLastName, 1, 37, 1,
                 1, '2000-01-01', @email, @phoneNumber, 'Dirección por definir', 1, 1, '00901',
                 @agencyId, @userId, GETDATE(), 1);
        END;

        SET @LogMessage = 'Paso 2 completado - Staff actualizado/insertado';
        RAISERROR(@LogMessage, 10, 1) WITH NOWAIT;

        -- 3. Actualizar roles si es necesario
        IF @roleNames IS NOT NULL AND LTRIM(RTRIM(@roleNames)) <> ''
        BEGIN
            -- Capturar roles actuales para auditoría (antes del DELETE)
            DECLARE @oldRoleNames NVARCHAR(MAX) = NULL;
            SELECT @oldRoleNames = STRING_AGG(r.Name, ',') WITHIN GROUP (ORDER BY r.Name)
            FROM AspNetUserRoles ur
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @userId;

            -- Eliminar roles existentes
            DELETE FROM AspNetUserRoles WHERE UserId = @userId;

            -- Insertar cada rol de la lista
            INSERT INTO AspNetUserRoles
                (UserId, RoleId, IsActive, CreatedAt)
            SELECT @userId, r.Id, 1, GETDATE()
            FROM AspNetRoles r
            INNER JOIN STRING_SPLIT(@roleNames, ',') ss ON LTRIM(RTRIM(ss.value)) = r.Name;

            -- Registrar en auditoría (ChangedBy: quien asigna o el propio usuario)
            DECLARE @auditOpId UNIQUEIDENTIFIER = NULL;
            DECLARE @changedBy NVARCHAR(450) = COALESCE(NULLIF(LTRIM(RTRIM(@assignedBy)), ''), @userId);
            EXEC [100_LogAuditChange]
                @TableName = 'AspNetUserRoles',
                @EntityId = @userId,
                @Action = 'UPDATE',
                @ChangedBy = @changedBy,
                @OldValues = @oldRoleNames,
                @NewValues = @roleNames,
                @BusinessContext = 'UserRolesUpdate',
                @OperationId = @auditOpId OUTPUT;
        END

        -- 4. Actualizar asignación de agencia (CRÍTICO: NO eliminar todas las asignaciones)
        IF @agencyId IS NOT NULL AND @agencyId > 0
        BEGIN
            -- Obtener el primer rol del usuario para calcular AgencyAssignmentType
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
                    IF @canBeOwner = 1
                    BEGIN
                        IF EXISTS (
                            SELECT 1
                            FROM AgencyUsers
                            WHERE AgencyId = @agencyId
                                AND AgencyAssignmentType = 'AGENCY_OWNER'
                                AND IsActive = 1
                                AND UserId != @userId
                        )
                        BEGIN
                            SET @agencyAssignmentType = 'AGENCY_STAFF';
                        END
                        ELSE
                        BEGIN
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
                    IF @userRoleName LIKE '%Coordinador%' OR @userRoleName LIKE '%Coordinator%'
                        SET @agencyAssignmentType = 'NUTRE_COORDINATOR';
                    ELSE IF @userRoleName LIKE '%Evaluador%' OR @userRoleName LIKE '%Evaluator%'
                        SET @agencyAssignmentType = 'NUTRE_EVALUATOR';
                    ELSE IF @userRoleName LIKE '%Admin%' OR @userRoleName LIKE '%Administrador%'
                        SET @agencyAssignmentType = 'NUTRE_ADMIN';
                    ELSE IF @userRoleName LIKE '%Contaduría%' OR @userRoleName LIKE '%Accounting%' OR @userRoleName LIKE '%Contable%'
                        SET @agencyAssignmentType = 'NUTRE_ACCOUNTING';
                    ELSE
                        SET @agencyAssignmentType = 'NUTRE_EVALUATOR';
                END
                ELSE
                BEGIN
                    SET @agencyAssignmentType = 'AGENCY_STAFF';
                END
            END
            ELSE
            BEGIN
                SET @agencyAssignmentType = 'AGENCY_STAFF';
            END

            IF EXISTS (
                SELECT 1
                FROM AgencyUsers
                WHERE UserId = @userId
                    AND AgencyId = @agencyId
                    AND IsActive = 1
            )
            BEGIN
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
                INSERT INTO AgencyUsers
                    (UserId, AgencyId, AgencyAssignmentType, IsActive, AssignedBy, CreatedAt, AssignedDate)
                VALUES
                    (@userId, @agencyId, @agencyAssignmentType, 1, @assignedBy, GETUTCDATE(), GETUTCDATE());
            END
        END

        -- 5. Sincronizar programa asignado al usuario (un solo programa desde Admin)
        IF @programId IS NULL OR @programId = 0
        BEGIN
            -- Quitar asignación de programa: desactivar filas en UserProgram
            UPDATE UserProgram
            SET IsActive = 0,
                UpdatedAt = GETUTCDATE()
            WHERE UserId = @userId;
        END
        ELSE
        BEGIN
            -- Validar que el programa existe
            IF EXISTS (SELECT 1 FROM Program WHERE Id = @programId AND IsActive = 1)
            BEGIN
                -- Desactivar otras asignaciones del usuario
                UPDATE UserProgram
                SET IsActive = 0,
                    UpdatedAt = GETUTCDATE()
                WHERE UserId = @userId;

                -- Insertar o reactivar la asignación al programa elegido
                IF EXISTS (SELECT 1 FROM UserProgram WHERE UserId = @userId AND ProgramId = @programId)
                BEGIN
                    UPDATE UserProgram
                    SET IsActive = 1,
                        UpdatedAt = GETUTCDATE()
                    WHERE UserId = @userId
                        AND ProgramId = @programId;
                END
                ELSE
                BEGIN
                    INSERT INTO UserProgram (UserId, ProgramId, IsActive, CreatedAt)
                    VALUES (@userId, @programId, 1, GETUTCDATE());
                END
            END
        END

        COMMIT TRANSACTION;

        SELECT 1 AS success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorLine INT = ERROR_LINE();

        SET @LogMessage = 'Error en SP 112_UpdateUser - Error: ' + @ErrorMessage + ', Line: ' + CAST(@ErrorLine AS NVARCHAR(10));
        RAISERROR(@LogMessage, 16, 1);

        SELECT
            0 AS success,
            @ErrorMessage AS errormessage,
            @ErrorNumber AS errornumber,
            @ErrorLine AS errorline;
    END CATCH
END;
GO
