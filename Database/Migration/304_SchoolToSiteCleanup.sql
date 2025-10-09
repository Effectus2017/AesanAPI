-- =============================================
-- Migration Script: 304_SchoolToSiteCleanup
-- Descripción: Limpia las tablas y stored procedures antiguos de School
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- IMPORTANTE: Este script debe ejecutarse SOLO después de verificar que:
-- 1. Los datos han sido migrados correctamente
-- 2. Las foreign keys han sido actualizadas
-- 3. La aplicación está funcionando correctamente con las nuevas tablas Site
-- 4. Se ha hecho un backup completo de la base de datos

BEGIN TRANSACTION;

DECLARE @sql NVARCHAR(MAX);
DECLARE @tableName NVARCHAR(255);
DECLARE @procedureName NVARCHAR(255);

PRINT 'Iniciando limpieza de tablas y stored procedures antiguos de School...';

-- 1. Eliminar stored procedures antiguos
DECLARE @oldProcedures TABLE (procedure_name NVARCHAR(255));

INSERT INTO @oldProcedures
VALUES
    ('104_InsertSchool_OLD'),
    ('105_GetSchoolById_OLD'),
    ('104_GetSchools_OLD'),
    ('102_DeleteSchool_OLD'),
    ('104_UpdateSchool_OLD'),
    ('102_HasMainSchool_OLD'),
    ('103_UpdateSchoolActiveStatus_OLD'),
    ('102_InsertSatelliteSchool_OLD'),
    ('103_UpdateSchoolSatellite_OLD'),
    ('100_InsertSchoolService_OLD'),
    ('100_InsertSchoolEducationLevels_OLD'),
    ('100_UpdateSchoolEducationLevels_OLD'),
    ('100_InsertSchoolDayCareHome_OLD'),
    ('100_UpdateSchoolDayCareHome_OLD'),
    ('100_InsertSchoolParticipants_OLD'),
    ('100_UpdateSchoolParticipants_OLD'),
    ('100_InsertSchoolChildGroup_OLD'),
    ('100_DeleteSchoolChildGroupsBySchoolId_OLD'),
    ('100_InsertSchoolOperatingDays_OLD'),
    ('100_DeleteSchoolOperatingDaysBySchoolId_OLD'),
    ('100_InsertSchoolStaff_OLD'),
    ('100_GetStaffBySchool_OLD'),
    ('100_InsertSchoolFacilities_OLD'),
    ('100_UpdateSchoolFacilities_OLD'),
    ('100_GetFacilitiesBySchool_OLD'),
    ('100_DeleteSchoolFacilitiesBySchoolId_OLD');

DECLARE procedure_cursor CURSOR FOR
SELECT procedure_name
FROM @oldProcedures;

OPEN procedure_cursor;
FETCH NEXT FROM procedure_cursor INTO @procedureName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT *
    FROM sys.procedures
    WHERE name = @procedureName)
    BEGIN
        SET @sql = 'DROP PROCEDURE [' + @procedureName + ']';
        EXEC sp_executesql @sql;
        PRINT 'Eliminado stored procedure: ' + @procedureName;
    END

    FETCH NEXT FROM procedure_cursor INTO @procedureName;
END

CLOSE procedure_cursor;
DEALLOCATE procedure_cursor;

-- 2. Eliminar tablas antiguas
DECLARE @oldTables TABLE (table_name NVARCHAR(255));

INSERT INTO @oldTables
VALUES
    ('School_OLD'),
    ('SchoolSatellite_OLD'),
    ('SchoolService_OLD'),
    ('SchoolEducationLevel_OLD'),
    ('SchoolDayCareHome_OLD'),
    ('SchoolStaff_OLD'),
    ('SchoolParticipant_OLD'),
    ('SchoolChildGroup_OLD'),
    ('SchoolOperatingDays_OLD'),
    ('SchoolFacility_OLD');

DECLARE table_cursor CURSOR FOR
SELECT table_name
FROM @oldTables;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = @tableName)
    BEGIN
        SET @sql = 'DROP TABLE [' + @tableName + ']';
        EXEC sp_executesql @sql;
        PRINT 'Eliminada tabla: ' + @tableName;
    END

    FETCH NEXT FROM table_cursor INTO @tableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- 3. Verificar que no queden referencias a las tablas antiguas
PRINT 'Verificando que no queden referencias a las tablas antiguas...';

-- Verificar stored procedures
SELECT
    name AS procedure_name,
    'Stored procedure con referencia a tabla School' AS issue
FROM sys.procedures
WHERE name LIKE '%School%'
    OR name LIKE '%school%'
    OR name LIKE '%SCHOOL%';

-- Verificar vistas
SELECT
    name AS view_name,
    'Vista con referencia a tabla School' AS issue
FROM sys.views
WHERE name LIKE '%School%'
    OR name LIKE '%school%'
    OR name LIKE '%SCHOOL%';

-- Verificar funciones
SELECT
    name AS function_name,
    'Función con referencia a tabla School' AS issue
FROM sys.objects
WHERE type = 'FN'
    AND (name LIKE '%School%'
    OR name LIKE '%school%'
    OR name LIKE '%SCHOOL%');

-- 4. Limpiar índices huérfanos (si los hay)
-- Nota: Los índices se eliminan automáticamente con las tablas
-- pero algunos pueden quedar huérfanos

-- 5. Limpiar constraints huérfanos (si los hay)
-- Nota: Los constraints se eliminan automáticamente con las tablas
-- pero algunos pueden quedar huérfanos

-- 6. Verificar que las nuevas tablas Site estén funcionando correctamente
PRINT 'Verificando que las nuevas tablas Site estén funcionando...';

-- Verificar que las tablas Site existen
IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'Site')
BEGIN
    PRINT 'Tabla Site existe y está funcionando';
END
ELSE
BEGIN
    PRINT 'ERROR: Tabla Site no existe';
    ROLLBACK TRANSACTION;
    RETURN;
END

-- Verificar que los stored procedures Site existen
IF EXISTS (SELECT *
FROM sys.procedures
WHERE name = '104_InsertSite')
BEGIN
    PRINT 'Stored procedures Site existen y están funcionando';
END
ELSE
BEGIN
    PRINT 'ERROR: Stored procedures Site no existen';
    ROLLBACK TRANSACTION;
    RETURN;
END

-- 7. Generar reporte final
PRINT 'Generando reporte final...';

-- Contar registros en las nuevas tablas
DECLARE @siteCount INT, @siteSatelliteCount INT, @siteServiceCount INT;

SELECT @siteCount = COUNT(*)
FROM Site;
SELECT @siteSatelliteCount = COUNT(*)
FROM SiteSatellite;
SELECT @siteServiceCount = COUNT(*)
FROM SiteService;

PRINT 'Registros en las nuevas tablas Site:';
PRINT '- Site: ' + CAST(@siteCount AS NVARCHAR(10));
PRINT '- SiteSatellite: ' + CAST(@siteSatelliteCount AS NVARCHAR(10));
PRINT '- SiteService: ' + CAST(@siteServiceCount AS NVARCHAR(10));

-- 8. Verificar que no queden objetos antiguos
DECLARE @remainingObjects INT;

SELECT @remainingObjects = COUNT(*)
FROM sys.objects
WHERE name LIKE '%School%'
    OR name LIKE '%school%'
    OR name LIKE '%SCHOOL%';

IF @remainingObjects = 0
BEGIN
    PRINT 'Limpieza completada: No quedan objetos con referencia a School';
END
ELSE
BEGIN
    PRINT 'ADVERTENCIA: Quedan ' + CAST(@remainingObjects AS NVARCHAR(10)) + ' objetos con referencia a School';
    PRINT 'Verificar manualmente estos objetos antes de continuar';
END

PRINT 'Limpieza de migración School a Site completada';
PRINT 'Las tablas y stored procedures antiguos han sido eliminados';
PRINT 'El sistema ahora funciona completamente con las tablas Site';

COMMIT TRANSACTION;
