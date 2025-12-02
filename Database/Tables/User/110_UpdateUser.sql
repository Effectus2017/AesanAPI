CREATE OR ALTER PROCEDURE [110_UpdateUser]
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
    SET @LogMessage = 'Iniciando SP UpdateUser - UserId: ' + @userId + ', Email: ' + @email;
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

        -- 4. Actualizar asignación de agencia
        IF @agencyId IS NOT NULL
        BEGIN
        -- Eliminar asignaciones existentes
        DELETE FROM AgencyUsers WHERE UserId = @userId;

            -- Determinar IsOwner e IsMonitor según el rol
            DECLARE @isOwner BIT = 0;
            DECLARE @isMonitor BIT = 0;
            
            -- Roles que pueden ser owners (según validación en 101_AssignAgencyToUser)
            IF @roleName IN ('Agency-Administrator', 'Agency-User')
            BEGIN
                SET @isOwner = 1;
                SET @isMonitor = 0;
            END
            -- Roles que pueden ser monitores (roles internos de AESAN/NUTRE)
            ELSE IF @roleName LIKE 'NUTRE_%' OR @roleName IN ('Monitor', 'Coordinador', 'Evaluador', 'Especialista', 'Nutrición', 'Contable', 'Director Contable', 'Abogado')
            BEGIN
                SET @isOwner = 0;
                SET @isMonitor = 1;
            END
            -- Para otros roles (Administrator, SuperAdministrator, etc.), no son ni owner ni monitor
            ELSE
            BEGIN
                SET @isOwner = 0;
                SET @isMonitor = 0;
            END

        -- Insertar nueva asignación
        INSERT INTO AgencyUsers
            (UserId, AgencyId, IsOwner, IsMonitor, IsActive, AssignedBy, CreatedAt)
        VALUES
                (@userId, @agencyId, @isOwner, @isMonitor, 1, @assignedBy, GETDATE());
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
        
        SET @LogMessage = 'Error en SP UpdateUser - Error: ' + @ErrorMessage + ', Line: ' + CAST(@ErrorLine AS NVARCHAR(10));
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
