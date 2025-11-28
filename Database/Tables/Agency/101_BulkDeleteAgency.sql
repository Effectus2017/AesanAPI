-- =============================================
-- Stored Procedure: 101_BulkDeleteAgency
-- Descripción: Elimina completamente una agencia y todas sus relaciones de datos (SOLO PARA DESARROLLO)
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================
-- IMPORTANTE: Este procedimiento solo debe usarse en ambiente de desarrollo
-- Elimina permanentemente todos los datos relacionados con la agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_BulkDeleteAgency]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    -- Asegurar que los errores causen rollback automático

    DECLARE @rowsAffected INT = 0;
    DECLARE @ownerUserId NVARCHAR(450) = NULL;
    DECLARE @tranCount INT = @@TRANCOUNT;
    DECLARE @deletedCount INT = 0;

    -- Solo iniciar transacción si no hay una activa
    IF @tranCount = 0
    BEGIN
        BEGIN TRANSACTION;
    END

    BEGIN TRY
        -- Verificar que la agencia existe
        IF NOT EXISTS (SELECT 1
    FROM Agency
    WHERE Id = @agencyId)
        BEGIN
        IF @tranCount = 0 AND @@TRANCOUNT > 0
            BEGIN
            ROLLBACK TRANSACTION;
        END
        RETURN 0;
    -- Agencia no encontrada
    END
        
        -- =============================================
        -- 1. Eliminar SiteExcursionExcludedService (relacionado con SiteExcursion)
        -- =============================================
        IF OBJECT_ID('SiteExcursionExcludedService', 'U') IS NOT NULL
        AND OBJECT_ID('SiteExcursion', 'U') IS NOT NULL
        BEGIN
        DELETE sees
            FROM SiteExcursionExcludedService sees
            INNER JOIN SiteExcursion se ON sees.SiteExcursionId = se.Id
            INNER JOIN Site s ON se.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
    END
        
        -- =============================================
        -- 2. Eliminar SiteExcursion (tiene ON DELETE CASCADE, pero incluirlo explícitamente)
        -- =============================================
        IF OBJECT_ID('SiteExcursion', 'U') IS NOT NULL
        BEGIN
        DELETE se
            FROM SiteExcursion se
            INNER JOIN Site s ON se.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
    END
        
        -- =============================================
        -- 3. Eliminar SchoolSite (relación entre School y Site)
        -- =============================================
        IF OBJECT_ID('SchoolSite', 'U') IS NOT NULL
        BEGIN
        DELETE ss
            FROM SchoolSite ss
            INNER JOIN Site s ON ss.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
    END
        
        -- =============================================
        -- 4. Eliminar SiteStaff (relación entre Site y Staff)
        -- =============================================
        IF OBJECT_ID('SiteStaff', 'U') IS NOT NULL
        BEGIN
        DELETE sst
            FROM SiteStaff sst
            INNER JOIN Site s ON sst.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
    END
        
        -- =============================================
        -- 5. Eliminar SitePersonInCharge (relación con Site)
        -- =============================================
        IF OBJECT_ID('SitePersonInCharge', 'U') IS NOT NULL
        BEGIN
        DELETE spic
            FROM SitePersonInCharge spic
            INNER JOIN Site s ON spic.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
    END
        
        -- =============================================
        -- 6. Eliminar Site (sitios de la agencia)
        -- =============================================
        DELETE FROM Site WHERE AgencyId = @agencyId;
        SET @deletedCount = @deletedCount + @@ROWCOUNT;
        
        -- =============================================
        -- 7. Eliminar School (escuelas de la agencia)
        -- =============================================
        DELETE FROM School WHERE AgencyId = @agencyId;
        SET @deletedCount = @deletedCount + @@ROWCOUNT;
        
        -- =============================================
        -- 8. Eliminar Staff (personal de la agencia)
        -- =============================================
        DELETE FROM Staff WHERE AgencyId = @agencyId;
        SET @deletedCount = @deletedCount + @@ROWCOUNT;
        
        -- =============================================
        -- 9. Eliminar AgencyFiles (archivos de la agencia)
        -- =============================================
        DELETE FROM AgencyFiles WHERE AgencyId = @agencyId;
        SET @deletedCount = @deletedCount + @@ROWCOUNT;
        
        -- =============================================
        -- 10. Eliminar AgencyProgram (programas de la agencia)
        -- =============================================
        DELETE FROM AgencyProgram WHERE AgencyId = @agencyId;
        SET @deletedCount = @deletedCount + @@ROWCOUNT;
        
        -- =============================================
        -- 11. Eliminar AgencyInscription (inscripción de la agencia)
        -- =============================================
        DELETE FROM AgencyInscription WHERE AgencyId = @agencyId;
        SET @deletedCount = @deletedCount + @@ROWCOUNT;
        
        -- =============================================
        -- 12. Obtener TODOS los UserIds relacionados con la agencia desde AgencyUsers ANTES de eliminar
        -- =============================================
        -- Crear tabla temporal para almacenar todos los UserIds a eliminar
        CREATE TABLE #UserIdsFromAgencyUsers
    (
        UserId NVARCHAR(450)
    );
        
        -- Obtener todos los usuarios relacionados con la agencia desde AgencyUsers
        INSERT INTO #UserIdsFromAgencyUsers
        (UserId)
    SELECT DISTINCT UserId
    FROM AgencyUsers
    WHERE AgencyId = @agencyId;
        
        -- Obtener el usuario creador (IsOwner = 1) para referencia
        SELECT TOP 1
        @ownerUserId = UserId
    FROM AgencyUsers
    WHERE AgencyId = @agencyId
        AND IsOwner = 1
        AND IsActive = 1;
        
        -- =============================================
        -- 13. Eliminar AgencyUsers (usuarios asignados a la agencia)
        -- =============================================
        DELETE FROM AgencyUsers WHERE AgencyId = @agencyId;
        SET @deletedCount = @deletedCount + @@ROWCOUNT;
        
        -- =============================================
        -- 14. Eliminar UserAgencyAssignment (asignaciones de usuarios)
        -- =============================================
        IF OBJECT_ID('UserAgencyAssignment', 'U') IS NOT NULL
        BEGIN
        DELETE FROM UserAgencyAssignment WHERE AgencyId = @agencyId;
    END
        
        -- =============================================
        -- 15. Eliminar AgencyUserAssignment (asignaciones de usuarios - tabla alternativa)
        -- =============================================
        IF OBJECT_ID('AgencyUserAssignment', 'U') IS NOT NULL
        BEGIN
        DELETE FROM AgencyUserAssignment WHERE AgencyId = @agencyId;
    END
        
        -- =============================================
        -- 16. Eliminar Staff que tiene UserId relacionado con la agencia (ANTES de eliminar usuarios)
        -- =============================================
        -- Eliminar Staff que tiene UserId de los usuarios relacionados con la agencia
        DELETE s FROM Staff s
        INNER JOIN #UserIdsFromAgencyUsers u ON s.UserId = u.UserId;
        
        -- =============================================
        -- 17. Eliminar usuarios de AspNetUsers relacionados con la agencia (obtenidos de AgencyUsers)
        -- =============================================
        -- Eliminar relaciones de todos los usuarios identificados desde AgencyUsers
        DELETE ur FROM AspNetUserRoles ur
        INNER JOIN #UserIdsFromAgencyUsers u ON ur.UserId = u.UserId;

        -- Eliminar los usuarios de AspNetUsers
        DELETE u FROM AspNetUsers u
        INNER JOIN #UserIdsFromAgencyUsers uid ON u.Id = uid.UserId;
        
        SET @deletedCount = @deletedCount + @@ROWCOUNT;

        -- Limpiar tabla temporal
        DROP TABLE #UserIdsFromAgencyUsers;
        
        -- =============================================
        -- 18. Eliminar Agency (finalmente, la agencia misma)
        -- =============================================
        -- Verificar si la agencia todavía existe antes de intentar eliminarla
        IF EXISTS (SELECT 1
    FROM Agency
    WHERE Id = @agencyId)
        BEGIN
        -- Intentar eliminar la agencia
        DELETE FROM Agency WHERE Id = @agencyId;

        SET @rowsAffected = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @rowsAffected;

        -- Verificar si se eliminó correctamente
        IF @rowsAffected > 0
            BEGIN
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
            -- La agencia existe pero no se pudo eliminar (probablemente por foreign keys)
            -- Verificar qué foreign keys están bloqueando
            DECLARE @fkError NVARCHAR(MAX) = 'No se pudo eliminar la agencia con ID ' + CAST(@agencyId AS NVARCHAR(10)) + '. ';

            -- Verificar foreign keys comunes
            IF EXISTS (SELECT 1
            FROM Site
            WHERE AgencyId = @agencyId)
                    SET @fkError = @fkError + 'Hay registros en Site. ';
            IF EXISTS (SELECT 1
            FROM School
            WHERE AgencyId = @agencyId)
                    SET @fkError = @fkError + 'Hay registros en School. ';
            IF EXISTS (SELECT 1
            FROM Staff
            WHERE AgencyId = @agencyId)
                    SET @fkError = @fkError + 'Hay registros en Staff. ';
            IF EXISTS (SELECT 1
            FROM AgencyFiles
            WHERE AgencyId = @agencyId)
                    SET @fkError = @fkError + 'Hay registros en AgencyFiles. ';
            IF EXISTS (SELECT 1
            FROM AgencyProgram
            WHERE AgencyId = @agencyId)
                    SET @fkError = @fkError + 'Hay registros en AgencyProgram. ';
            IF EXISTS (SELECT 1
            FROM AgencyInscription
            WHERE AgencyId = @agencyId)
                    SET @fkError = @fkError + 'Hay registros en AgencyInscription. ';
            IF EXISTS (SELECT 1
            FROM AgencyUsers
            WHERE AgencyId = @agencyId)
                    SET @fkError = @fkError + 'Hay registros en AgencyUsers. ';

            SET @fkError = @fkError + 'Total de registros eliminados antes del error: ' + CAST(@deletedCount AS NVARCHAR(10));

            -- Hacer ROLLBACK de todo porque no se completó la operación
            IF @tranCount = 0 AND @@TRANCOUNT > 0
                BEGIN
                ROLLBACK TRANSACTION;
            END
            RAISERROR(@fkError, 16, 1);
            RETURN -1;
        -- Error
        END
    END
        ELSE
        BEGIN
        -- La agencia no existe (ya fue eliminada - posiblemente en un intento anterior)
        -- Hacer COMMIT de las eliminaciones que sí se hicieron
        IF @tranCount = 0
            BEGIN
            COMMIT TRANSACTION;
        END
        RETURN 1;
    -- Éxito (aunque la agencia ya no existía, se limpiaron sus relaciones)
    END
    END TRY
    BEGIN CATCH
        -- Solo hacer ROLLBACK si iniciamos la transacción
        IF @tranCount = 0 AND @@TRANCOUNT > 0
        BEGIN
        ROLLBACK TRANSACTION;
    END
        
        -- Registrar el error
        DECLARE @errorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        -- Relanzar el error
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
        
        RETURN -1; -- Error
    END CATCH
END;
GO