-- =============================================
-- Script de Prueba: Site Migration
-- Descripción: Pruebas para verificar que la migración de School a Site funcionó correctamente
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- IMPORTANTE: Ejecutar este script después de ejecutar:
-- 1. 300_SchoolToSiteMigration.sql
-- 2. 305_AdditionalSiteStoredProcedures.sql

BEGIN TRANSACTION TestSiteMigration;

BEGIN TRY
    PRINT 'Iniciando pruebas de migración de Site...';
    
    -- =============================================
    -- PRUEBA 1: Verificar que las tablas Site existen
    -- =============================================
    
    PRINT 'PRUEBA 1: Verificando existencia de tablas Site...';
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'Site')
    BEGIN
    RAISERROR('ERROR: Tabla Site no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteSatellite')
    BEGIN
    RAISERROR('ERROR: Tabla SiteSatellite no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteService')
    BEGIN
    RAISERROR('ERROR: Tabla SiteService no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteEducationLevel')
    BEGIN
    RAISERROR('ERROR: Tabla SiteEducationLevel no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteDayCareHome')
    BEGIN
    RAISERROR('ERROR: Tabla SiteDayCareHome no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteStaff')
    BEGIN
    RAISERROR('ERROR: Tabla SiteStaff no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteParticipant')
    BEGIN
    RAISERROR('ERROR: Tabla SiteParticipant no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteChildGroup')
    BEGIN
    RAISERROR('ERROR: Tabla SiteChildGroup no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteOperatingDays')
    BEGIN
    RAISERROR('ERROR: Tabla SiteOperatingDays no existe', 16, 1);
END
    
    PRINT 'PRUEBA 1 COMPLETADA: Todas las tablas Site existen';
    
    -- =============================================
    -- PRUEBA 2: Verificar que los Stored Procedures existen
    -- =============================================
    
    PRINT 'PRUEBA 2: Verificando existencia de Stored Procedures...';
    
    -- Stored Procedures principales
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '104_InsertSite')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 104_InsertSite no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '105_GetSiteById')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 105_GetSiteById no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '104_GetSites')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 104_GetSites no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '102_DeleteSite')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 102_DeleteSite no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '104_UpdateSite')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 104_UpdateSite no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '102_HasMainSite')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 102_HasMainSite no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '103_UpdateSiteActiveStatus')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 103_UpdateSiteActiveStatus no existe', 16, 1);
END
    
    -- Stored Procedures adicionales
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteOperatingDays')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 100_InsertSiteOperatingDays no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_GetSiteOperatingDays')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 100_GetSiteOperatingDays no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_ToggleSiteOperatingDay')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 100_ToggleSiteOperatingDay no existe', 16, 1);
END
    
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_GetNextSiteNumber')
    BEGIN
    RAISERROR('ERROR: Stored Procedure 100_GetNextSiteNumber no existe', 16, 1);
END
    
    PRINT 'PRUEBA 2 COMPLETADA: Todos los Stored Procedures existen';
    
    -- =============================================
    -- PRUEBA 3: Verificar integridad de datos
    -- =============================================
    
    PRINT 'PRUEBA 3: Verificando integridad de datos...';
    
    -- Verificar que los datos se migraron correctamente
    DECLARE @SchoolCount INT = (SELECT COUNT(*)
FROM School);
    DECLARE @SiteCount INT = (SELECT COUNT(*)
FROM Site);
    
    IF @SchoolCount != @SiteCount
    BEGIN
    RAISERROR('ERROR: Conteo de datos no coincide. School: %d, Site: %d', 16, 1, @SchoolCount, @SiteCount);
END
    
    -- Verificar satélites
    DECLARE @SchoolSatelliteCount INT = (SELECT COUNT(*)
FROM SchoolSatellite);
    DECLARE @SiteSatelliteCount INT = (SELECT COUNT(*)
FROM SiteSatellite);
    
    IF @SchoolSatelliteCount != @SiteSatelliteCount
    BEGIN
    RAISERROR('ERROR: Conteo de satélites no coincide. SchoolSatellite: %d, SiteSatellite: %d', 16, 1, @SchoolSatelliteCount, @SiteSatelliteCount);
END
    
    -- Verificar servicios
    DECLARE @SchoolServiceCount INT = (SELECT COUNT(*)
FROM SchoolService);
    DECLARE @SiteServiceCount INT = (SELECT COUNT(*)
FROM SiteService);
    
    IF @SchoolServiceCount != @SiteServiceCount
    BEGIN
    RAISERROR('ERROR: Conteo de servicios no coincide. SchoolService: %d, SiteService: %d', 16, 1, @SchoolServiceCount, @SiteServiceCount);
END
    
    -- Verificar niveles educativos
    DECLARE @SchoolEducationLevelCount INT = (SELECT COUNT(*)
FROM SchoolEducationLevel);
    DECLARE @SiteEducationLevelCount INT = (SELECT COUNT(*)
FROM SiteEducationLevel);
    
    IF @SchoolEducationLevelCount != @SiteEducationLevelCount
    BEGIN
    RAISERROR('ERROR: Conteo de niveles educativos no coincide. SchoolEducationLevel: %d, SiteEducationLevel: %d', 16, 1, @SchoolEducationLevelCount, @SiteEducationLevelCount);
END
    
    -- Verificar Day Care Home
    DECLARE @SchoolDayCareHomeCount INT = (SELECT COUNT(*)
FROM SchoolDayCareHome);
    DECLARE @SiteDayCareHomeCount INT = (SELECT COUNT(*)
FROM SiteDayCareHome);
    
    IF @SchoolDayCareHomeCount != @SiteDayCareHomeCount
    BEGIN
    RAISERROR('ERROR: Conteo de Day Care Home no coincide. SchoolDayCareHome: %d, SiteDayCareHome: %d', 16, 1, @SchoolDayCareHomeCount, @SiteDayCareHomeCount);
END
    
    -- Verificar Staff
    DECLARE @SchoolStaffCount INT = (SELECT COUNT(*)
FROM SchoolStaff);
    DECLARE @SiteStaffCount INT = (SELECT COUNT(*)
FROM SiteStaff);
    
    IF @SchoolStaffCount != @SiteStaffCount
    BEGIN
    RAISERROR('ERROR: Conteo de Staff no coincide. SchoolStaff: %d, SiteStaff: %d', 16, 1, @SchoolStaffCount, @SiteStaffCount);
END
    
    -- Verificar participantes
    DECLARE @SchoolParticipantCount INT = (SELECT COUNT(*)
FROM SchoolParticipant);
    DECLARE @SiteParticipantCount INT = (SELECT COUNT(*)
FROM SiteParticipant);
    
    IF @SchoolParticipantCount != @SiteParticipantCount
    BEGIN
    RAISERROR('ERROR: Conteo de participantes no coincide. SchoolParticipant: %d, SiteParticipant: %d', 16, 1, @SchoolParticipantCount, @SiteParticipantCount);
END
    
    -- Verificar grupos de niños
    DECLARE @SchoolChildGroupCount INT = (SELECT COUNT(*)
FROM SchoolChildGroup);
    DECLARE @SiteChildGroupCount INT = (SELECT COUNT(*)
FROM SiteChildGroup);
    
    IF @SchoolChildGroupCount != @SiteChildGroupCount
    BEGIN
    RAISERROR('ERROR: Conteo de grupos de niños no coincide. SchoolChildGroup: %d, SiteChildGroup: %d', 16, 1, @SchoolChildGroupCount, @SiteChildGroupCount);
END
    
    -- Verificar días de funcionamiento
    DECLARE @SchoolOperatingDaysCount INT = (SELECT COUNT(*)
FROM SchoolOperatingDays);
    DECLARE @SiteOperatingDaysCount INT = (SELECT COUNT(*)
FROM SiteOperatingDays);
    
    IF @SchoolOperatingDaysCount != @SiteOperatingDaysCount
    BEGIN
    RAISERROR('ERROR: Conteo de días de funcionamiento no coincide. SchoolOperatingDays: %d, SiteOperatingDays: %d', 16, 1, @SchoolOperatingDaysCount, @SiteOperatingDaysCount);
END
    
    PRINT 'PRUEBA 3 COMPLETADA: Integridad de datos verificada';
    
    -- =============================================
    -- PRUEBA 4: Probar Stored Procedures
    -- =============================================
    
    PRINT 'PRUEBA 4: Probando Stored Procedures...';
    
    -- Probar GetSites (no devuelve valor de retorno, solo ejecuta)
    BEGIN TRY
        EXEC [dbo].[104_GetSites] @take = 10, @skip = 0, @name = NULL, @cityId = NULL, @regionId = NULL, @agencyId = NULL, @alls = 1;
        PRINT 'Stored Procedure 104_GetSites ejecutado correctamente';
    END TRY
    BEGIN CATCH
        RAISERROR('ERROR: Stored Procedure 104_GetSites falló', 16, 1);
    END CATCH
    
    -- Probar GetSiteById si hay sitios
    IF @SiteCount > 0
    BEGIN
    DECLARE @FirstSiteId INT = (SELECT TOP 1
        Id
    FROM Site);
    BEGIN TRY
            EXEC [dbo].[105_GetSiteById] @id = @FirstSiteId;
            PRINT 'Stored Procedure 105_GetSiteById ejecutado correctamente';
        END TRY
        BEGIN CATCH
            RAISERROR('ERROR: Stored Procedure 105_GetSiteById falló', 16, 1);
        END CATCH
END
    
    -- Probar HasMainSite
    BEGIN TRY
        EXEC [dbo].[102_HasMainSite];
        PRINT 'Stored Procedure 102_HasMainSite ejecutado correctamente';
    END TRY
    BEGIN CATCH
        RAISERROR('ERROR: Stored Procedure 102_HasMainSite falló', 16, 1);
    END CATCH
    
    PRINT 'PRUEBA 4 COMPLETADA: Stored Procedures funcionan correctamente';
    
    -- =============================================
    -- PRUEBA 5: Verificar Foreign Keys
    -- =============================================
    
    PRINT 'PRUEBA 5: Verificando Foreign Keys...';
    
    -- Verificar que existen foreign keys en Site (no verificamos nombres específicos ya que pueden variar)
    DECLARE @SiteFKCount INT = (SELECT COUNT(*)
FROM sys.foreign_keys fk
    INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'Site');
    
    IF @SiteFKCount = 0
    BEGIN
    RAISERROR('ERROR: No se encontraron Foreign Keys en la tabla Site', 16, 1);
END
    
    -- Verificar que existen foreign keys en SiteSatellite
    DECLARE @SiteSatelliteFKCount INT = (SELECT COUNT(*)
FROM sys.foreign_keys fk
    INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'SiteSatellite');
    
    IF @SiteSatelliteFKCount = 0
    BEGIN
    RAISERROR('ERROR: No se encontraron Foreign Keys en la tabla SiteSatellite', 16, 1);
END
    
    -- Verificar que existen foreign keys en SiteService
    DECLARE @SiteServiceFKCount INT = (SELECT COUNT(*)
FROM sys.foreign_keys fk
    INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'SiteService');
    
    IF @SiteServiceFKCount = 0
    BEGIN
    RAISERROR('ERROR: No se encontraron Foreign Keys en la tabla SiteService', 16, 1);
END
    
    PRINT 'PRUEBA 5 COMPLETADA: Foreign Keys verificadas';
    
    -- =============================================
    -- PRUEBA 6: Verificar Índices
    -- =============================================
    
    PRINT 'PRUEBA 6: Verificando Índices...';
    
    -- Verificar que existen índices en Site
    DECLARE @SiteIndexCount INT = (SELECT COUNT(*)
FROM sys.indexes i
    INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'Site' AND i.name IS NOT NULL);
    
    IF @SiteIndexCount = 0
    BEGIN
    RAISERROR('ERROR: No se encontraron índices en la tabla Site', 16, 1);
END
    
    -- Verificar que existen índices en SiteSatellite
    DECLARE @SiteSatelliteIndexCount INT = (SELECT COUNT(*)
FROM sys.indexes i
    INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'SiteSatellite' AND i.name IS NOT NULL);
    
    IF @SiteSatelliteIndexCount = 0
    BEGIN
    RAISERROR('ERROR: No se encontraron índices en la tabla SiteSatellite', 16, 1);
END
    
    -- Verificar que existe índice único en SiteSatellite (SatelliteSiteId)
    IF NOT EXISTS (SELECT *
FROM sys.indexes i
    INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'SiteSatellite' AND i.is_unique = 1)
    BEGIN
    RAISERROR('ERROR: No se encontró índice único en SiteSatellite', 16, 1);
END
    
    PRINT 'PRUEBA 6 COMPLETADA: Índices verificados';
    
    -- =============================================
    -- COMPLETAR PRUEBAS
    -- =============================================
    
    COMMIT TRANSACTION TestSiteMigration;
    
    PRINT '=============================================';
    PRINT 'TODAS LAS PRUEBAS COMPLETADAS EXITOSAMENTE';
    PRINT '=============================================';
    PRINT 'Resumen de verificación:';
    PRINT '- Tablas Site: ✓';
    PRINT '- Stored Procedures: ✓';
    PRINT '- Integridad de datos: ✓';
    PRINT '- Funcionalidad de SPs: ✓';
    PRINT '- Foreign Keys: ✓';
    PRINT '- Índices: ✓';
    PRINT '=============================================';
    PRINT 'La migración de School a Site fue exitosa.';
    PRINT 'El sistema está listo para usar las nuevas tablas Site.';
    PRINT '=============================================';

END
TRY
BEGIN CATCH
-- En caso de error, hacer rollback
IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION TestSiteMigration;

-- Mostrar información del error
DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
DECLARE @ErrorState INT = ERROR_STATE();

PRINT '=============================================';
PRINT 'ERROR EN LAS PRUEBAS DE MIGRACIÓN';
PRINT '=============================================';
PRINT 'Mensaje: ' + @ErrorMessage;
PRINT 'Severidad: ' + CAST(@ErrorSeverity AS VARCHAR(10));
PRINT 'Estado: ' + CAST(@ErrorState AS VARCHAR(10));
PRINT '=============================================';
PRINT 'Las pruebas han fallado.';
PRINT 'Revisar el error y corregir la migración.';
PRINT '=============================================';

-- Re-lanzar el error
RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
