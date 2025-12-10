-- =============================================
-- Stored Procedure: 101_BulkDeleteUser
-- Descripción: Elimina completamente un usuario de AspNetUsers y todas sus relaciones de datos, incluyendo la agencia si es propietario
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================
-- IMPORTANTE: Este procedimiento elimina permanentemente todos los datos relacionados con el usuario
-- Si el usuario es propietario de una agencia (IsOwner = 1 en AgencyUsers), también elimina la agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_BulkDeleteUser]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    -- Asegurar que los errores causen rollback automático

    DECLARE @rowsAffected INT = 0;
    DECLARE @isAgencyOwner BIT = 0;
    DECLARE @agencyId INT = NULL;
    DECLARE @staffId INT = NULL;
    DECLARE @tranCount INT = @@TRANCOUNT;
    DECLARE @deletedCount INT = 0;
    DECLARE @currentRows INT = 0;

    -- Solo iniciar transacción si no hay una activa
    IF @tranCount = 0
    BEGIN
        BEGIN TRANSACTION;
    END

    BEGIN TRY
        PRINT '========================================';
        PRINT 'INICIANDO ELIMINACIÓN DE USUARIO ID: ' + @userId;
        PRINT '========================================';
        PRINT '';

        -- Verificar que el usuario existe
        IF NOT EXISTS (SELECT 1
    FROM AspNetUsers
    WHERE Id = @userId)
        BEGIN
        IF @tranCount = 0 AND @@TRANCOUNT > 0
            BEGIN
            ROLLBACK TRANSACTION;
        END
        PRINT 'ERROR: Usuario con ID ' + @userId + ' no encontrado.';
        RETURN 0;
    -- Usuario no encontrado
    END

        -- Obtener StaffId si existe
        IF EXISTS (SELECT 1
    FROM Staff
    WHERE UserId = @userId)
        BEGIN
        SELECT TOP 1
            @staffId = Id
        FROM Staff
        WHERE UserId = @userId;
    END

        -- Verificar si el usuario es propietario de una agencia y obtener AgencyId
        IF EXISTS (
            SELECT 1
    FROM AgencyUsers
    WHERE UserId = @userId
        AND IsOwner = 1
        AND IsActive = 1
        )
        BEGIN
        SET @isAgencyOwner = 1;
        SELECT TOP 1
            @agencyId = AgencyId
        FROM AgencyUsers
        WHERE UserId = @userId
            AND IsOwner = 1
            AND IsActive = 1;
        PRINT '⚠ El usuario es propietario de la agencia ID ' + CAST(@agencyId AS NVARCHAR(10)) + '. Se eliminará la agencia completa.';
        PRINT '';
    END

        -- =============================================
        -- 1. Si el usuario es propietario de una agencia, eliminar relaciones de StaffRelationship primero
        -- =============================================
        IF @isAgencyOwner = 1 AND @agencyId IS NOT NULL
        BEGIN
        PRINT '';
        PRINT '--- Eliminando relaciones de StaffRelationship antes de eliminar agencia ---';

        -- Crear tabla temporal con todos los StaffIds relacionados con la agencia
        CREATE TABLE #StaffIdsToDelete
        (
            StaffId INT
        );

        -- Obtener todos los StaffIds de la agencia
        INSERT INTO #StaffIdsToDelete
            (StaffId)
        SELECT Id
        FROM Staff
        WHERE AgencyId = @agencyId;

        -- Obtener StaffIds adicionales que tienen UserId relacionado con usuarios de la agencia
        INSERT INTO #StaffIdsToDelete
            (StaffId)
        SELECT DISTINCT s.Id
        FROM Staff s
            INNER JOIN AgencyUsers au ON s.UserId = au.UserId
        WHERE au.AgencyId = @agencyId
            AND s.Id NOT IN (SELECT StaffId
            FROM #StaffIdsToDelete);

        -- Eliminar StaffRelationship donde estos Staff son el principal (StaffId)
        IF OBJECT_ID('StaffRelationship', 'U') IS NOT NULL AND EXISTS (SELECT 1
            FROM #StaffIdsToDelete)
        BEGIN
            DELETE sr FROM StaffRelationship sr
                INNER JOIN #StaffIdsToDelete s ON sr.StaffId = s.StaffId;
            SET @currentRows = @@ROWCOUNT;
            IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffRelationship (como StaffId)';
        END

        -- Eliminar StaffRelationship donde estos Staff son el relacionado (RelatedStaffId)
        IF OBJECT_ID('StaffRelationship', 'U') IS NOT NULL AND EXISTS (SELECT 1
            FROM #StaffIdsToDelete)
        BEGIN
            DELETE sr FROM StaffRelationship sr
                INNER JOIN #StaffIdsToDelete s ON sr.RelatedStaffId = s.StaffId;
            SET @currentRows = @@ROWCOUNT;
            IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffRelationship (como RelatedStaffId)';
        END

        -- Limpiar tabla temporal
        DROP TABLE #StaffIdsToDelete;

        PRINT '';
        PRINT '--- Eliminando agencia completa (Usuario es propietario) ---';
        -- Llamar al procedimiento de eliminación de agencia
        -- NOTA: Este procedimiento ya elimina el usuario, Staff y todas las relaciones
        EXEC [dbo].[101_BulkDeleteAgency] @agencyId = @agencyId;
        SET @deletedCount = @deletedCount + 1;
        -- Contar la agencia eliminada
        PRINT '✓ Agencia ID ' + CAST(@agencyId AS NVARCHAR(10)) + ' eliminada completamente';
        PRINT '';
        PRINT '========================================';
        PRINT 'ELIMINACIÓN COMPLETADA EXITOSAMENTE';
        PRINT 'Total de registros eliminados: ' + CAST(@deletedCount AS NVARCHAR(10));
        PRINT '========================================';

        -- Solo hacer COMMIT si iniciamos la transacción
        IF @tranCount = 0
            BEGIN
            COMMIT TRANSACTION;
        END
        RETURN 1;
    -- Éxito (el usuario ya fue eliminado por 101_BulkDeleteAgency)
    END

        -- =============================================
        -- 2. Eliminar Staff asociado al usuario (si NO se eliminó la agencia)
        -- PRIMERO para evitar conflictos de FK antes de eliminar el usuario
        -- =============================================
        IF @staffId IS NOT NULL
        BEGIN
        PRINT '';
        PRINT '--- Eliminando Staff asociado al usuario ---';
        -- Llamar al procedimiento de eliminación de Staff
        EXEC [dbo].[101_BulkDeleteStaff] @staffId = @staffId;
        SET @deletedCount = @deletedCount + 1;
        -- Contar el Staff eliminado
        PRINT '✓ Staff ID ' + CAST(@staffId AS NVARCHAR(10)) + ' eliminado completamente';
        PRINT '';
        -- NOTA: Si el Staff tenía UserId, 101_BulkDeleteStaff ya eliminó el usuario
        -- Verificar si el usuario todavía existe
        IF NOT EXISTS (SELECT 1
        FROM AspNetUsers
        WHERE Id = @userId)
            BEGIN
            PRINT '⚠ El usuario ya fue eliminado por 101_BulkDeleteStaff.';
            PRINT '';
            PRINT '========================================';
            PRINT 'ELIMINACIÓN COMPLETADA EXITOSAMENTE';
            PRINT 'Total de registros eliminados: ' + CAST(@deletedCount AS NVARCHAR(10));
            PRINT '========================================';

            -- Solo hacer COMMIT si iniciamos la transacción
            IF @tranCount = 0
                BEGIN
                COMMIT TRANSACTION;
            END
            RETURN 1;
        -- Éxito (el usuario ya fue eliminado por 101_BulkDeleteStaff)
        END
    END

        -- =============================================
        -- 3. Eliminar relaciones del usuario (si NO se eliminó la agencia)
        -- =============================================
        PRINT '';
        PRINT '--- Eliminando relaciones del usuario ---';

        -- Eliminar AspNetUserRoles (roles del usuario)
        IF OBJECT_ID('AspNetUserRoles', 'U') IS NOT NULL
        BEGIN
        DELETE FROM AspNetUserRoles 
            WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserRoles';
    END

        -- Eliminar AspNetUserClaims (claims del usuario)
        IF OBJECT_ID('AspNetUserClaims', 'U') IS NOT NULL
        BEGIN
        DELETE FROM AspNetUserClaims 
            WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserClaims';
    END

        -- Eliminar AspNetUserLogins (logins externos del usuario)
        IF OBJECT_ID('AspNetUserLogins', 'U') IS NOT NULL
        BEGIN
        DELETE FROM AspNetUserLogins 
            WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserLogins';
    END

        -- Eliminar UserPermission (permisos del usuario)
        IF OBJECT_ID('UserPermission', 'U') IS NOT NULL
        BEGIN
        DELETE FROM UserPermission 
            WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserPermission';
    END

        -- Eliminar UserProgram (programas asignados al usuario)
        IF OBJECT_ID('UserProgram', 'U') IS NOT NULL
        BEGIN
        DELETE FROM UserProgram 
            WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserProgram';
    END

        -- Eliminar TemporaryPasswords (contraseñas temporales del usuario)
        IF OBJECT_ID('TemporaryPasswords', 'U') IS NOT NULL
        BEGIN
        DELETE FROM TemporaryPasswords 
            WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de TemporaryPasswords';
    END

        -- =============================================
        -- 4. Eliminar AgencyUsers (donde UserId = @userId)
        -- =============================================
        DELETE FROM AgencyUsers 
        WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUsers (donde UserId)';

        -- =============================================
        -- 5. Eliminar AgencyUsers (donde AssignedBy = @userId)
        -- =============================================
        DELETE FROM AgencyUsers 
        WHERE AssignedBy = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUsers (donde AssignedBy)';

        -- =============================================
        -- 6. Eliminar UserAgencyAssignment (donde UserId = @userId)
        -- =============================================
        IF OBJECT_ID('UserAgencyAssignment', 'U') IS NOT NULL
        BEGIN
        DELETE FROM UserAgencyAssignment 
            WHERE UserId = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserAgencyAssignment (donde UserId)';
    END

        -- =============================================
        -- 7. Eliminar UserAgencyAssignment (donde AssignedBy = @userId)
        -- =============================================
        IF OBJECT_ID('UserAgencyAssignment', 'U') IS NOT NULL
        BEGIN
        DELETE FROM UserAgencyAssignment 
            WHERE AssignedBy = @userId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserAgencyAssignment (donde AssignedBy)';
    END

        -- =============================================
        -- 8. Eliminar AspNetUsers (finalmente, el usuario mismo)
        -- =============================================
        PRINT '';
        PRINT '--- Eliminando el usuario ---';
        
        IF EXISTS (SELECT 1
    FROM AspNetUsers
    WHERE Id = @userId)
        BEGIN
        DELETE FROM AspNetUsers WHERE Id = @userId;
        SET @rowsAffected = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @rowsAffected;

        -- Verificar si se eliminó correctamente
        IF @rowsAffected > 0
            BEGIN
            PRINT '✓ Eliminado el usuario con ID ' + @userId;
            PRINT '';
            PRINT '========================================';
            PRINT 'ELIMINACIÓN COMPLETADA EXITOSAMENTE';
            PRINT 'Total de registros eliminados: ' + CAST(@deletedCount AS NVARCHAR(10));
            PRINT '========================================';

            -- Solo hacer COMMIT si iniciamos la transacción
            IF @tranCount = 0
                BEGIN
                COMMIT TRANSACTION;
            END
            RETURN 1;
        -- Éxito
        END
            ELSE
            BEGIN
            -- El usuario existe pero no se pudo eliminar (probablemente por foreign keys)
            DECLARE @fkError NVARCHAR(MAX) = 'No se pudo eliminar el usuario con ID ' + @userId + '. ';

            -- Verificar foreign keys comunes
            IF EXISTS (SELECT 1
            FROM Staff
            WHERE UserId = @userId)
                    SET @fkError = @fkError + 'Hay registros en Staff. ';
            IF EXISTS (SELECT 1
            FROM AgencyUsers
            WHERE UserId = @userId)
                    SET @fkError = @fkError + 'Hay registros en AgencyUsers. ';
            IF EXISTS (SELECT 1
            FROM AspNetUserRoles
            WHERE UserId = @userId)
                    SET @fkError = @fkError + 'Hay registros en AspNetUserRoles. ';

            SET @fkError = @fkError + 'Total de registros eliminados antes del error: ' + CAST(@deletedCount AS NVARCHAR(10));

            PRINT '';
            PRINT '========================================';
            PRINT 'ERROR: No se pudo eliminar el usuario';
            PRINT '========================================';
            PRINT @fkError;
            PRINT '========================================';

            -- Hacer ROLLBACK de todo porque no se completó la operación
            IF @tranCount = 0 AND @@TRANCOUNT > 0
                BEGIN
                ROLLBACK TRANSACTION;
                PRINT 'Transacción revertida (ROLLBACK).';
            END
            RAISERROR(@fkError, 16, 1);
            RETURN -1;
        -- Error
        END
    END
        ELSE
        BEGIN
        -- El usuario no existe (ya fue eliminado - posiblemente en un intento anterior)
        PRINT '⚠ El usuario con ID ' + @userId + ' ya no existe, pero se limpiaron sus relaciones.';
        PRINT '';
        PRINT '========================================';
        PRINT 'ELIMINACIÓN COMPLETADA';
        PRINT 'Total de registros eliminados: ' + CAST(@deletedCount AS NVARCHAR(10));
        PRINT '========================================';

        -- Hacer COMMIT de las eliminaciones que sí se hicieron
        IF @tranCount = 0
            BEGIN
            COMMIT TRANSACTION;
        END
        RETURN 1;
    -- Éxito (aunque el usuario ya no existía, se limpiaron sus relaciones)
    END
    END TRY
    BEGIN CATCH
        PRINT '';
        PRINT '========================================';
        PRINT 'ERROR DURANTE LA ELIMINACIÓN';
        PRINT '========================================';
        
        -- Solo hacer ROLLBACK si iniciamos la transacción
        IF @tranCount = 0 AND @@TRANCOUNT > 0
        BEGIN
        ROLLBACK TRANSACTION;
        PRINT 'Transacción revertida (ROLLBACK).';
    END
        
        -- Registrar el error
        DECLARE @errorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        PRINT 'Mensaje de error: ' + @errorMessage;
        PRINT 'Registros eliminados antes del error: ' + CAST(@deletedCount AS NVARCHAR(10));
        PRINT '========================================';
        
        -- Relanzar el error
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
        
        RETURN -1; -- Error
    END CATCH
END;
GO
