-- =============================================
-- Migration Script: 301_SchoolToSiteTableRename
-- Descripción: Renombra las tablas de School a Site
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- IMPORTANTE: Este script debe ejecutarse DESPUÉS de crear las nuevas tablas Site
-- y migrar los datos desde las tablas School existentes

BEGIN TRANSACTION;

-- Renombrar tablas principales
IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'School')
BEGIN
    EXEC sp_rename 'School', 'School_OLD';
    PRINT 'Tabla School renombrada a School_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolSatellite')
BEGIN
    EXEC sp_rename 'SchoolSatellite', 'SchoolSatellite_OLD';
    PRINT 'Tabla SchoolSatellite renombrada a SchoolSatellite_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolService')
BEGIN
    EXEC sp_rename 'SchoolService', 'SchoolService_OLD';
    PRINT 'Tabla SchoolService renombrada a SchoolService_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolEducationLevel')
BEGIN
    EXEC sp_rename 'SchoolEducationLevel', 'SchoolEducationLevel_OLD';
    PRINT 'Tabla SchoolEducationLevel renombrada a SchoolEducationLevel_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolDayCareHome')
BEGIN
    EXEC sp_rename 'SchoolDayCareHome', 'SchoolDayCareHome_OLD';
    PRINT 'Tabla SchoolDayCareHome renombrada a SchoolDayCareHome_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolStaff')
BEGIN
    EXEC sp_rename 'SchoolStaff', 'SchoolStaff_OLD';
    PRINT 'Tabla SchoolStaff renombrada a SchoolStaff_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolParticipant')
BEGIN
    EXEC sp_rename 'SchoolParticipant', 'SchoolParticipant_OLD';
    PRINT 'Tabla SchoolParticipant renombrada a SchoolParticipant_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolChildGroup')
BEGIN
    EXEC sp_rename 'SchoolChildGroup', 'SchoolChildGroup_OLD';
    PRINT 'Tabla SchoolChildGroup renombrada a SchoolChildGroup_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolOperatingDays')
BEGIN
    EXEC sp_rename 'SchoolOperatingDays', 'SchoolOperatingDays_OLD';
    PRINT 'Tabla SchoolOperatingDays renombrada a SchoolOperatingDays_OLD';
END

IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolFacility')
BEGIN
    EXEC sp_rename 'SchoolFacility', 'SchoolFacility_OLD';
    PRINT 'Tabla SchoolFacility renombrada a SchoolFacility_OLD';
END

-- Renombrar stored procedures
DECLARE @sql NVARCHAR(MAX);
DECLARE @old_name NVARCHAR(255);
DECLARE @new_name NVARCHAR(255);

-- Lista de stored procedures a renombrar
DECLARE @procedures TABLE (
    old_name NVARCHAR(255),
    new_name NVARCHAR(255)
);

INSERT INTO @procedures
VALUES
    ('104_InsertSchool', '104_InsertSchool_OLD'),
    ('105_GetSchoolById', '105_GetSchoolById_OLD'),
    ('104_GetSchools', '104_GetSchools_OLD'),
    ('102_DeleteSchool', '102_DeleteSchool_OLD'),
    ('104_UpdateSchool', '104_UpdateSchool_OLD'),
    ('102_HasMainSchool', '102_HasMainSchool_OLD'),
    ('103_UpdateSchoolActiveStatus', '103_UpdateSchoolActiveStatus_OLD'),
    ('102_InsertSatelliteSchool', '102_InsertSatelliteSchool_OLD'),
    ('103_UpdateSchoolSatellite', '103_UpdateSchoolSatellite_OLD'),
    ('100_InsertSchoolService', '100_InsertSchoolService_OLD'),
    ('100_InsertSchoolEducationLevels', '100_InsertSchoolEducationLevels_OLD'),
    ('100_UpdateSchoolEducationLevels', '100_UpdateSchoolEducationLevels_OLD'),
    ('100_InsertSchoolDayCareHome', '100_InsertSchoolDayCareHome_OLD'),
    ('100_UpdateSchoolDayCareHome', '100_UpdateSchoolDayCareHome_OLD'),
    ('100_InsertSchoolParticipants', '100_InsertSchoolParticipants_OLD'),
    ('100_UpdateSchoolParticipants', '100_UpdateSchoolParticipants_OLD'),
    ('100_InsertSchoolChildGroup', '100_InsertSchoolChildGroup_OLD'),
    ('100_DeleteSchoolChildGroupsBySchoolId', '100_DeleteSchoolChildGroupsBySchoolId_OLD'),
    ('100_InsertSchoolOperatingDays', '100_InsertSchoolOperatingDays_OLD'),
    ('100_DeleteSchoolOperatingDaysBySchoolId', '100_DeleteSchoolOperatingDaysBySchoolId_OLD'),
    ('100_InsertSchoolStaff', '100_InsertSchoolStaff_OLD'),
    ('100_GetStaffBySchool', '100_GetStaffBySchool_OLD'),
    ('100_InsertSchoolFacilities', '100_InsertSchoolFacilities_OLD'),
    ('100_UpdateSchoolFacilities', '100_UpdateSchoolFacilities_OLD'),
    ('100_GetFacilitiesBySchool', '100_GetFacilitiesBySchool_OLD'),
    ('100_DeleteSchoolFacilitiesBySchoolId', '100_DeleteSchoolFacilitiesBySchoolId_OLD');

-- Renombrar cada stored procedure
DECLARE procedure_cursor CURSOR FOR
SELECT old_name, new_name
FROM @procedures;

OPEN procedure_cursor;
FETCH NEXT FROM procedure_cursor INTO @old_name, @new_name;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT *
    FROM sys.procedures
    WHERE name = @old_name)
    BEGIN
        SET @sql = 'EXEC sp_rename ''' + @old_name + ''', ''' + @new_name + '''';
        EXEC sp_executesql @sql;
        PRINT 'Stored procedure ' + @old_name + ' renombrado a ' + @new_name;
    END

    FETCH NEXT FROM procedure_cursor INTO @old_name, @new_name;
END

CLOSE procedure_cursor;
DEALLOCATE procedure_cursor;

-- Renombrar índices
-- Nota: Los índices se renombrarán automáticamente con las tablas
-- pero algunos pueden necesitar renombrado manual

-- Renombrar constraints
-- Nota: Los constraints se renombrarán automáticamente con las tablas
-- pero algunos pueden necesitar renombrado manual

PRINT 'Migración de tablas School a Site completada';
PRINT 'Las tablas originales han sido renombradas con sufijo _OLD';
PRINT 'Las nuevas tablas Site están listas para uso';

COMMIT TRANSACTION;
