-- =============================================
-- Stored Procedure: 101_BulkDeleteStaff
-- Descripción: Elimina completamente un Staff y todas sus relaciones de datos, incluyendo la agencia si es propietario
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================
-- IMPORTANTE: Este procedimiento elimina permanentemente todos los datos relacionados con el Staff
-- Si el Staff es propietario de una agencia (IsOwner = 1 en AgencyUsers), también elimina la agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_BulkDeleteStaff]
    @staffId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    -- Asegurar que los errores causen rollback automático

    DECLARE @rowsAffected INT = 0;
    DECLARE @staffUserId NVARCHAR(450) = NULL;
    DECLARE @staffAgencyId INT = NULL;
    DECLARE @isAgencyOwner BIT = 0;
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
        PRINT 'INICIANDO ELIMINACIÓN DE STAFF ID: ' + CAST(@staffId AS NVARCHAR(10));
        PRINT '========================================';
        PRINT '';

        -- Verificar que el Staff existe y obtener información relevante
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId)
        BEGIN
        IF @tranCount = 0 AND @@TRANCOUNT > 0
            BEGIN
            ROLLBACK TRANSACTION;
        END
        PRINT 'ERROR: Staff con ID ' + CAST(@staffId AS NVARCHAR(10)) + ' no encontrado.';
        RETURN 0;
    -- Staff no encontrado
    END

        -- Obtener información del Staff antes de eliminarlo
        SELECT
        @staffUserId = UserId,
        @staffAgencyId = AgencyId
    FROM Staff
    WHERE Id = @staffId;

        -- Verificar si el Staff es propietario de la agencia
        IF @staffUserId IS NOT NULL AND @staffAgencyId IS NOT NULL
        BEGIN
        IF EXISTS (
                SELECT 1
        FROM AgencyUsers
        WHERE UserId = @staffUserId
            AND AgencyId = @staffAgencyId
            AND IsOwner = 1
            AND IsActive = 1
            )
            BEGIN
            SET @isAgencyOwner = 1;
            PRINT '⚠ El Staff es propietario de la agencia ID ' + CAST(@staffAgencyId AS NVARCHAR(10)) + '. Se eliminará la agencia completa.';
            PRINT '';
        END
    END

        -- =============================================
        -- 1. Eliminar StaffRelationship donde el Staff es el principal (StaffId)
        -- =============================================
        IF OBJECT_ID('StaffRelationship', 'U') IS NOT NULL
        BEGIN
        DELETE FROM StaffRelationship 
            WHERE StaffId = @staffId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffRelationship (como StaffId)';
    END

        -- =============================================
        -- 2. Eliminar StaffRelationship donde el Staff es el relacionado (RelatedStaffId)
        -- =============================================
        IF OBJECT_ID('StaffRelationship', 'U') IS NOT NULL
        BEGIN
        DELETE FROM StaffRelationship 
            WHERE RelatedStaffId = @staffId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffRelationship (como RelatedStaffId)';
    END

        -- =============================================
        -- 3. Eliminar SiteStaff (asignaciones de staff a sitios)
        -- =============================================
        IF OBJECT_ID('SiteStaff', 'U') IS NOT NULL
        BEGIN
        DELETE FROM SiteStaff 
            WHERE StaffId = @staffId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteStaff';
    END

        -- =============================================
        -- 4. Si el Staff es propietario de una agencia, eliminar la agencia completa
        -- =============================================
        IF @isAgencyOwner = 1 AND @staffAgencyId IS NOT NULL
        BEGIN
        PRINT '';
        PRINT '--- Eliminando agencia completa (Staff es propietario) ---';
        -- Llamar al procedimiento de eliminación de agencia
        EXEC [dbo].[101_BulkDeleteAgency] @agencyId = @staffAgencyId;
        SET @deletedCount = @deletedCount + 1;
        -- Contar la agencia eliminada
        PRINT '✓ Agencia ID ' + CAST(@staffAgencyId AS NVARCHAR(10)) + ' eliminada completamente';
        PRINT '';
    END
        ELSE
        BEGIN
        -- Si no es propietario pero tiene agencia, solo eliminar la relación AgencyUsers
        IF @staffUserId IS NOT NULL AND @staffAgencyId IS NOT NULL
            BEGIN
            DELETE FROM AgencyUsers 
                WHERE UserId = @staffUserId
                AND AgencyId = @staffAgencyId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUsers (relación con agencia)';
        END
    END

        -- =============================================
        -- 5. Eliminar Staff PRIMERO (antes del usuario para evitar conflictos de FK)
        -- =============================================
        PRINT '';
        PRINT '--- Eliminando el Staff ---';
        
        IF EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId)
        BEGIN
        DELETE FROM Staff WHERE Id = @staffId;
        SET @rowsAffected = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @rowsAffected;

        IF @rowsAffected = 0
            BEGIN
            -- El Staff existe pero no se pudo eliminar (probablemente por foreign keys)
            DECLARE @fkError NVARCHAR(MAX) = 'No se pudo eliminar el Staff con ID ' + CAST(@staffId AS NVARCHAR(10)) + '. ';

            -- Verificar foreign keys comunes
            IF EXISTS (SELECT 1
            FROM StaffRelationship
            WHERE StaffId = @staffId OR RelatedStaffId = @staffId)
                    SET @fkError = @fkError + 'Hay registros en StaffRelationship. ';
            IF EXISTS (SELECT 1
            FROM SiteStaff
            WHERE StaffId = @staffId)
                    SET @fkError = @fkError + 'Hay registros en SiteStaff. ';

            SET @fkError = @fkError + 'Total de registros eliminados antes del error: ' + CAST(@deletedCount AS NVARCHAR(10));

            PRINT '';
            PRINT '========================================';
            PRINT 'ERROR: No se pudo eliminar el Staff';
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
            ELSE
            BEGIN
            PRINT '✓ Eliminado el Staff con ID ' + CAST(@staffId AS NVARCHAR(10));
        END
    END
        ELSE
        BEGIN
        -- El Staff no existe (ya fue eliminado - posiblemente en un intento anterior)
        PRINT '⚠ El Staff con ID ' + CAST(@staffId AS NVARCHAR(10)) + ' ya no existe, pero se limpiaron sus relaciones.';
    END

        -- =============================================
        -- 6. Si el Staff tenía UserId y NO se eliminó la agencia, eliminar relaciones del usuario
        -- (Si se eliminó la agencia, el usuario ya fue eliminado por 101_BulkDeleteAgency)
        -- IMPORTANTE: Eliminar el Staff antes del usuario evita conflictos de FK
        -- IMPORTANTE: Eliminar TODAS las relaciones del usuario antes de eliminar el usuario
        -- =============================================
        IF @staffUserId IS NOT NULL AND @isAgencyOwner = 0
        BEGIN
        PRINT '';
        PRINT '--- Eliminando datos relacionados con el usuario del Staff ---';

        -- Eliminar UserPermission (permisos del usuario) - PRIMERO para evitar FK constraint
        IF OBJECT_ID('UserPermission', 'U') IS NOT NULL
            BEGIN
            DELETE FROM UserPermission 
                WHERE UserId = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserPermission';
        END

        -- Eliminar relaciones de roles del usuario
        IF OBJECT_ID('AspNetUserRoles', 'U') IS NOT NULL
            BEGIN
            DELETE FROM AspNetUserRoles 
                WHERE UserId = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserRoles';
        END

        -- Eliminar AspNetUserClaims (claims del usuario)
        IF OBJECT_ID('AspNetUserClaims', 'U') IS NOT NULL
            BEGIN
            DELETE FROM AspNetUserClaims 
                WHERE UserId = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserClaims';
        END

        -- Eliminar AspNetUserLogins (logins externos del usuario)
        IF OBJECT_ID('AspNetUserLogins', 'U') IS NOT NULL
            BEGIN
            DELETE FROM AspNetUserLogins 
                WHERE UserId = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserLogins';
        END

        -- Eliminar UserProgram (programas asignados al usuario)
        IF OBJECT_ID('UserProgram', 'U') IS NOT NULL
            BEGIN
            DELETE FROM UserProgram 
                WHERE UserId = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserProgram';
        END

        -- Eliminar TemporaryPasswords (contraseñas temporales del usuario)
        IF OBJECT_ID('TemporaryPasswords', 'U') IS NOT NULL
            BEGIN
            DELETE FROM TemporaryPasswords 
                WHERE UserId = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de TemporaryPasswords';
        END

        -- Eliminar UserAgencyAssignment (donde UserId = @staffUserId)
        IF OBJECT_ID('UserAgencyAssignment', 'U') IS NOT NULL
            BEGIN
            DELETE FROM UserAgencyAssignment 
                WHERE UserId = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserAgencyAssignment (donde UserId)';
        END

        -- Eliminar UserAgencyAssignment (donde AssignedBy = @staffUserId)
        IF OBJECT_ID('UserAgencyAssignment', 'U') IS NOT NULL
            BEGIN
            DELETE FROM UserAgencyAssignment 
                WHERE AssignedBy = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserAgencyAssignment (donde AssignedBy)';
        END

        -- Eliminar otras relaciones de AgencyUsers (si no se eliminó la agencia)
        DELETE FROM AgencyUsers 
            WHERE UserId = @staffUserId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUsers';

        -- Eliminar AgencyUsers (donde AssignedBy = @staffUserId)
        DELETE FROM AgencyUsers 
            WHERE AssignedBy = @staffUserId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
                PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUsers (donde AssignedBy)';

        -- Eliminar el usuario de AspNetUsers (ahora es seguro porque todas las relaciones fueron eliminadas)
        IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
            BEGIN
            DELETE FROM AspNetUsers 
                WHERE Id = @staffUserId;
            SET @currentRows = @@ROWCOUNT;
            SET @deletedCount = @deletedCount + @currentRows;
            IF @currentRows > 0
                    PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUsers';
        END
    END

        -- =============================================
        -- 7. Finalizar y hacer COMMIT
        -- =============================================
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
        RETURN 1; -- Éxito
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

EXEC [dbo].[101_BulkDeleteStaff] 1;