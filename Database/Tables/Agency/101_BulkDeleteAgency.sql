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

    -- Protección: No permitir eliminar agencias propietarias (isPropietary = 1)
    IF EXISTS (SELECT 1 FROM Agency WHERE Id = @agencyId AND IsPropietary = 1)
    BEGIN
        RAISERROR('No se puede eliminar una agencia propietaria. Esta agencia está protegida y no puede ser eliminada bajo ninguna circunstancia.', 16, 1);
        RETURN 0;
    END

    DECLARE @rowsAffected INT = 0;
    DECLARE @ownerUserId NVARCHAR(450) = NULL;
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
        PRINT 'INICIANDO ELIMINACIÓN DE AGENCIA ID: ' + CAST(@agencyId AS NVARCHAR(10));
        PRINT '========================================';
        PRINT '';

        -- Verificar que la agencia existe
        IF NOT EXISTS (SELECT 1
    FROM Agency
    WHERE Id = @agencyId)
        BEGIN
        IF @tranCount = 0 AND @@TRANCOUNT > 0
            BEGIN
            ROLLBACK TRANSACTION;
        END
        PRINT 'ERROR: Agencia con ID ' + CAST(@agencyId AS NVARCHAR(10)) + ' no encontrada.';
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
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteExcursionExcludedService';
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
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteExcursion';
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
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SchoolSite';
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
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteStaff';
    END
        
        -- =============================================
        -- 4.1. Eliminar SiteOperatingDaysOfWeek (días de la semana de operación) - PRIMERO
        -- IMPORTANTE: Debe eliminarse ANTES de Site para evitar FK constraint
        -- =============================================
        PRINT '';
        PRINT '--- Eliminando SiteOperatingDaysOfWeek ---';
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SiteOperatingDaysOfWeek' AND schema_id = SCHEMA_ID('dbo'))
        BEGIN
        DELETE FROM dbo.SiteOperatingDaysOfWeek
        WHERE SiteId IN (SELECT Id FROM dbo.Site WHERE AgencyId = @agencyId);
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteOperatingDaysOfWeek';
        ELSE
            PRINT '⚠ No se encontraron registros de SiteOperatingDaysOfWeek para eliminar';
    END
        ELSE
        BEGIN
        PRINT '⚠ Tabla SiteOperatingDaysOfWeek no existe, omitiendo eliminación';
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
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SitePersonInCharge';
    END
        
        -- =============================================
        -- 5.2. Eliminar SiteOperatingDayService (servicios de días operativos - debe ir antes de SiteOperatingDays)
        -- =============================================
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SiteOperatingDayService' AND schema_id = SCHEMA_ID('dbo'))
        BEGIN
        DELETE FROM dbo.SiteOperatingDayService
        WHERE OperatingDayId IN (
            SELECT sod.Id 
            FROM dbo.SiteOperatingDays sod
            INNER JOIN dbo.Site s ON sod.SiteId = s.Id
            WHERE s.AgencyId = @agencyId
        );
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteOperatingDayService';
    END
        
        -- =============================================
        -- 5.3. Eliminar SiteOperatingDays (días específicos de operación)
        -- =============================================
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SiteOperatingDays' AND schema_id = SCHEMA_ID('dbo'))
        BEGIN
        DELETE FROM dbo.SiteOperatingDays
        WHERE SiteId IN (SELECT Id FROM dbo.Site WHERE AgencyId = @agencyId);
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteOperatingDays';
    END
        
        -- =============================================
        -- 5.4. Eliminar SiteProgram (programas del sitio)
        -- =============================================
        IF OBJECT_ID('SiteProgram', 'U') IS NOT NULL
        BEGIN
        DELETE sp
            FROM SiteProgram sp
            INNER JOIN Site s ON sp.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteProgram';
    END
        
        -- =============================================
        -- 5.5. Eliminar SiteParticipant (participantes del sitio)
        -- =============================================
        IF OBJECT_ID('SiteParticipant', 'U') IS NOT NULL
        BEGIN
        DELETE sp
            FROM SiteParticipant sp
            INNER JOIN Site s ON sp.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteParticipant';
    END
        
        -- =============================================
        -- 5.6. Eliminar SiteFacility (instalaciones del sitio)
        -- =============================================
        IF OBJECT_ID('SiteFacility', 'U') IS NOT NULL
        BEGIN
        DELETE sf
            FROM SiteFacility sf
            INNER JOIN Site s ON sf.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteFacility';
    END
        
        -- =============================================
        -- 5.7. Eliminar SiteEducationLevel (niveles educativos del sitio)
        -- =============================================
        IF OBJECT_ID('SiteEducationLevel', 'U') IS NOT NULL
        BEGIN
        DELETE sel
            FROM SiteEducationLevel sel
            INNER JOIN Site s ON sel.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteEducationLevel';
    END
        
        -- =============================================
        -- 5.8. Eliminar SiteChildGroup (grupos de niños del sitio)
        -- =============================================
        IF OBJECT_ID('SiteChildGroup', 'U') IS NOT NULL
        BEGIN
        DELETE scg
            FROM SiteChildGroup scg
            INNER JOIN Site s ON scg.SiteId = s.Id
            WHERE s.AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de SiteChildGroup';
    END
        
        -- =============================================
        -- 6. Eliminar Site (sitios de la agencia)
        -- =============================================
        DELETE FROM Site WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de Site';
        
        -- =============================================
        -- 7. Eliminar School (escuelas de la agencia)
        -- =============================================
        DELETE FROM School WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de School';
        
        -- =============================================
        -- 8. Crear tabla temporal con todos los StaffIds de la agencia
        -- =============================================
        CREATE TABLE #StaffIdsFromAgency
    (
        StaffId INT
    );
        
        -- Obtener todos los StaffIds de la agencia
        INSERT INTO #StaffIdsFromAgency
        (StaffId)
    SELECT Id
    FROM Staff
    WHERE AgencyId = @agencyId;
        
        -- =============================================
        -- 9. Eliminar StaffRelationship donde el Staff es el principal (StaffId)
        -- =============================================
        IF OBJECT_ID('StaffRelationship', 'U') IS NOT NULL
        BEGIN
        DELETE sr FROM StaffRelationship sr
            INNER JOIN #StaffIdsFromAgency s ON sr.StaffId = s.StaffId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffRelationship (como StaffId)';
    END
        
        -- =============================================
        -- 10. Eliminar StaffRelationship donde el Staff es el relacionado (RelatedStaffId)
        -- =============================================
        IF OBJECT_ID('StaffRelationship', 'U') IS NOT NULL
        BEGIN
        DELETE sr FROM StaffRelationship sr
            INNER JOIN #StaffIdsFromAgency s ON sr.RelatedStaffId = s.StaffId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffRelationship (como RelatedStaffId)';
    END
        
        -- =============================================
        -- 10b. Eliminar StaffContractByClassification (contratos por clasificación del staff)
        -- =============================================
        IF OBJECT_ID('StaffContractByClassification', 'U') IS NOT NULL
        BEGIN
        DELETE scc FROM StaffContractByClassification scc
            INNER JOIN #StaffIdsFromAgency s ON scc.StaffId = s.StaffId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffContractByClassification';
    END
        
        -- =============================================
        -- 11. Eliminar Staff (personal de la agencia)
        -- =============================================
        DELETE FROM Staff WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de Staff';
        
        -- Limpiar tabla temporal
        DROP TABLE #StaffIdsFromAgency;
        
        -- =============================================
        -- 12. Eliminar AgencyFiles (archivos de la agencia)
        -- =============================================
        DELETE FROM AgencyFiles WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyFiles';
        
        -- =============================================
        -- 13. Eliminar AgencyProgram (programas de la agencia)
        -- =============================================
        DELETE FROM AgencyProgram WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyProgram';
        
        -- =============================================
        -- 14. Eliminar AgencyInscription (inscripción de la agencia)
        -- =============================================
        DELETE FROM AgencyInscription WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyInscription';
        
        -- =============================================
        -- 14b. Eliminar AgencyStatusHistory (historial de cambios de estado de la agencia)
        -- =============================================
        IF OBJECT_ID('AgencyStatusHistory', 'U') IS NOT NULL
        BEGIN
        DELETE FROM AgencyStatusHistory WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyStatusHistory';
    END
        
        -- =============================================
        -- 15. Obtener TODOS los UserIds relacionados con la agencia desde AgencyUsers ANTES de eliminar
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
        
        -- Obtener el usuario creador (AgencyAssignmentType = 'AGENCY_OWNER') para referencia
        SELECT TOP 1
        @ownerUserId = UserId
    FROM AgencyUsers
    WHERE AgencyId = @agencyId
        AND AgencyAssignmentType = 'AGENCY_OWNER'
        AND IsActive = 1;
        
        -- =============================================
        -- 15.1. Eliminar AgencyUsers donde AssignedBy apunta a usuarios que se van a eliminar
        -- IMPORTANTE: Debe hacerse ANTES de eliminar AgencyUsers por AgencyId
        -- =============================================
        DELETE FROM AgencyUsers 
        WHERE AssignedBy IN (SELECT UserId FROM #UserIdsFromAgencyUsers);
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUsers (donde AssignedBy apunta a usuarios a eliminar)';
        
        -- =============================================
        -- 16. Eliminar AgencyUsers (usuarios asignados a la agencia)
        -- =============================================
        DELETE FROM AgencyUsers WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUsers';
        
        -- =============================================
        -- 17. Eliminar UserAgencyAssignment (asignaciones de usuarios)
        -- =============================================
        IF OBJECT_ID('UserAgencyAssignment', 'U') IS NOT NULL
        BEGIN
        DELETE FROM UserAgencyAssignment WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserAgencyAssignment';
    END
        
        -- =============================================
        -- 18. Eliminar AgencyUserAssignment (asignaciones de usuarios - tabla alternativa)
        -- =============================================
        IF OBJECT_ID('AgencyUserAssignment', 'U') IS NOT NULL
        BEGIN
        DELETE FROM AgencyUserAssignment WHERE AgencyId = @agencyId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyUserAssignment';
    END
        
        -- =============================================
        -- 18b. Eliminar StaffContractByClassification de Staff relacionados por UserId (antes de eliminar Staff)
        -- =============================================
        IF OBJECT_ID('StaffContractByClassification', 'U') IS NOT NULL
        BEGIN
        DELETE scc FROM StaffContractByClassification scc
            INNER JOIN Staff s ON scc.StaffId = s.Id
            INNER JOIN #UserIdsFromAgencyUsers u ON s.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de StaffContractByClassification (Staff por UserId)';
    END
        
        -- =============================================
        -- 19. Eliminar Staff que tiene UserId relacionado con la agencia (ANTES de eliminar usuarios)
        -- =============================================
        -- Eliminar Staff que tiene UserId de los usuarios relacionados con la agencia
        DELETE s FROM Staff s
        INNER JOIN #UserIdsFromAgencyUsers u ON s.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros adicionales de Staff (por UserId)';
        
        -- =============================================
        -- 20. Eliminar usuarios de AspNetUsers relacionados con la agencia (obtenidos de AgencyUsers)
        -- =============================================
        PRINT '';
        PRINT '--- Eliminando datos relacionados con usuarios ---';
        
        -- Eliminar AgencyStatusHistory donde ChangedBy son usuarios a eliminar (FK ChangedBy -> AspNetUsers)
        IF OBJECT_ID('AgencyStatusHistory', 'U') IS NOT NULL
        BEGIN
        DELETE FROM AgencyStatusHistory
            WHERE ChangedBy IN (SELECT UserId FROM #UserIdsFromAgencyUsers);
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AgencyStatusHistory (ChangedBy)';
    END
        
        -- Eliminar UserPermission (permisos de usuarios)
        IF OBJECT_ID('UserPermission', 'U') IS NOT NULL
        BEGIN
        DELETE up FROM UserPermission up
            INNER JOIN #UserIdsFromAgencyUsers u ON up.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserPermission';
    END

        -- Eliminar UserProgram (programas asignados a usuarios)
        IF OBJECT_ID('UserProgram', 'U') IS NOT NULL
        BEGIN
        DELETE up FROM UserProgram up
            INNER JOIN #UserIdsFromAgencyUsers u ON up.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de UserProgram';
    END

        -- Eliminar TemporaryPasswords (contraseñas temporales)
        IF OBJECT_ID('TemporaryPasswords', 'U') IS NOT NULL
        BEGIN
        DELETE tp FROM TemporaryPasswords tp
            INNER JOIN #UserIdsFromAgencyUsers u ON tp.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de TemporaryPasswords';
    END

        -- Eliminar EmailLog (logs de correo de los usuarios)
        IF OBJECT_ID('EmailLog', 'U') IS NOT NULL
        BEGIN
        DELETE el FROM EmailLog el
            INNER JOIN #UserIdsFromAgencyUsers u ON el.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de EmailLog';
    END

        -- Eliminar AspNetUserClaims (claims de usuarios)
        IF OBJECT_ID('AspNetUserClaims', 'U') IS NOT NULL
        BEGIN
        DELETE auc FROM AspNetUserClaims auc
            INNER JOIN #UserIdsFromAgencyUsers u ON auc.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserClaims';
    END

        -- Eliminar AspNetUserLogins (logins externos de usuarios)
        IF OBJECT_ID('AspNetUserLogins', 'U') IS NOT NULL
        BEGIN
        DELETE aul FROM AspNetUserLogins aul
            INNER JOIN #UserIdsFromAgencyUsers u ON aul.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserLogins';
    END

        -- Eliminar relaciones de todos los usuarios identificados desde AgencyUsers
        DELETE ur FROM AspNetUserRoles ur
        INNER JOIN #UserIdsFromAgencyUsers u ON ur.UserId = u.UserId;
        SET @currentRows = @@ROWCOUNT;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUserRoles';

        -- Eliminar los usuarios de AspNetUsers
        DELETE u FROM AspNetUsers u
        INNER JOIN #UserIdsFromAgencyUsers uid ON u.Id = uid.UserId;
        SET @currentRows = @@ROWCOUNT;
        SET @deletedCount = @deletedCount + @currentRows;
        IF @currentRows > 0
            PRINT '✓ Eliminados ' + CAST(@currentRows AS NVARCHAR(10)) + ' registros de AspNetUsers';

        -- Limpiar tabla temporal
        DROP TABLE #UserIdsFromAgencyUsers;
        
        -- =============================================
        -- 21. Eliminar Agency (finalmente, la agencia misma)
        -- =============================================
        PRINT '';
        PRINT '--- Eliminando la agencia ---';
        
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
            PRINT '✓ Eliminada la agencia con ID ' + CAST(@agencyId AS NVARCHAR(10));
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
        PRINT '⚠ La agencia con ID ' + CAST(@agencyId AS NVARCHAR(10)) + ' ya no existe, pero se limpiaron sus relaciones.';
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
    -- Éxito (aunque la agencia ya no existía, se limpiaron sus relaciones)
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

--EXEC [dbo].[101_BulkDeleteAgency] 1;