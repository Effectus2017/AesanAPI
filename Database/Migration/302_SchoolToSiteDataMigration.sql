-- =============================================
-- Migration Script: 302_SchoolToSiteDataMigration
-- Descripción: Migra datos de las tablas School a Site
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- IMPORTANTE: Este script debe ejecutarse DESPUÉS de crear las nuevas tablas Site
-- y ANTES de renombrar las tablas School existentes

BEGIN TRANSACTION;

DECLARE @RowsMigrated INT = 0;
DECLARE @ErrorMessage NVARCHAR(MAX);

PRINT 'Iniciando migración de datos de School a Site...';

-- 1. Migrar datos de la tabla principal School a Site
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'School') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'Site')
BEGIN
    INSERT INTO Site
        (
        AgencyId, Name, StartDate, Address, CityId, RegionId, ZipCode, Latitude, Longitude,
        PostalAddress, PostalCityId, PostalRegionId, PostalZipCode, SameAsPhysicalAddress,
        OrganizationTypeId, CenterTypeId, NonProfit, BaseYear, RenewalYear, OperatingFromDate, OperatingToDate, OperatingDaysCalculated,
        KitchenTypeId, GroupTypeId, DeliveryTypeId, SponsorTypeId, ApplicantTypeId, ResidentialTypeId, OperatingPolicyId, AreaTypeId, LocationTypeId,
        HasWarehouse, HasDiningRoom, SitePhone, Extension, MobilePhone,
        CommunityId, WalkersId, SiteTypeId, ExperienceId, ReviewResultId, ReviewDate, ReviewJustification,
        SiteCode, SiteLocationId, GeneralEnrollment, SiteNumber, ServiceTime,
        OrganizedAthleticPrograms, AtRiskService, IsMainSite, IsActive, InactiveJustification, InactiveDate, CreatedAt, UpdatedAt
        )
    SELECT
        AgencyId, Name, StartDate, Address, CityId, RegionId, ZipCode, Latitude, Longitude,
        PostalAddress, PostalCityId, PostalRegionId, PostalZipCode, SameAsPhysicalAddress,
        OrganizationTypeId, CenterTypeId, NonProfit, BaseYear, RenewalYear, OperatingFromDate, OperatingToDate, OperatingDaysCalculated,
        KitchenTypeId, GroupTypeId, DeliveryTypeId, SponsorTypeId, ApplicantTypeId, ResidentialTypeId, OperatingPolicyId, AreaTypeId, LocationTypeId,
        HasWarehouse, HasDiningRoom, SitePhone, Extension, MobilePhone,
        CommunityId, WalkersId, SiteTypeId, ExperienceId, ReviewResultId, ReviewDate, ReviewJustification,
        SiteCode, SiteLocationId, GeneralEnrollment, SiteNumber, ServiceTime,
        -- Campos que no existen en School, usar valores por defecto
        0 AS OrganizedAthleticPrograms, 0 AS AtRiskService,
        -- Determinar IsMainSite basado en SiteNumber = 1
        CASE WHEN SiteNumber = 1 THEN 1 ELSE 0 END AS IsMainSite,
        IsActive, InactiveJustification, InactiveDate, CreatedAt, UpdatedAt
    FROM School;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de School a Site';
END
ELSE
BEGIN
    SET @ErrorMessage = 'Error: Las tablas School o Site no existen';
    PRINT @ErrorMessage;
    ROLLBACK TRANSACTION;
    RETURN;
END

-- 2. Migrar datos de SchoolSatellite a SiteSatellite
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolSatellite') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteSatellite')
BEGIN
    INSERT INTO SiteSatellite
        (
        MainSiteId, SatelliteSiteId, AssignmentDate, Comment, IsActive, CreatedAt, UpdatedAt
        )
    SELECT
        MainSchoolId, SatelliteSchoolId, AssignmentDate, Comment, IsActive, CreatedAt, UpdatedAt
    FROM SchoolSatellite;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolSatellite a SiteSatellite';
END

-- 3. Migrar datos de SchoolService a SiteService
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolService') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteService')
BEGIN
    INSERT INTO SiteService
        (
        SiteId, ChildGroupId, Breakfast, BreakfastFrom, BreakfastTo, Lunch, LunchFrom, LunchTo,
        SnackAM, SnackAMFrom, SnackAMTo, Dinner, DinnerFrom, DinnerTo, SnackPM, SnackPMFrom, SnackPMTo,
        SnackNight, SnackNightFrom, SnackNightTo, DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo,
        DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo, SnackExtended, SnackExtendedFrom, SnackExtendedTo,
        SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, ChildGroupId, Breakfast, BreakfastFrom, BreakfastTo, Lunch, LunchFrom, LunchTo,
        SnackAM, SnackAMFrom, SnackAMTo, Dinner, DinnerFrom, DinnerTo, SnackPM, SnackPMFrom, SnackPMTo,
        SnackNight, SnackNightFrom, SnackNightTo, DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo,
        DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo, SnackExtended, SnackExtendedFrom, SnackExtendedTo,
        SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo, CreatedAt, UpdatedAt
    FROM SchoolService;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolService a SiteService';
END

-- 4. Migrar datos de SchoolEducationLevel a SiteEducationLevel
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolEducationLevel') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteEducationLevel')
BEGIN
    INSERT INTO SiteEducationLevel
        (
        SiteId, EducationLevelId, IsActive, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, EducationLevelId, IsActive, CreatedAt, UpdatedAt
    FROM SchoolEducationLevel;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolEducationLevel a SiteEducationLevel';
END

-- 5. Migrar datos de SchoolDayCareHome a SiteDayCareHome
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolDayCareHome') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteDayCareHome')
BEGIN
    INSERT INTO SiteDayCareHome
        (
        SiteId, IsAuthorizedToOperate, HasFamilyDepartmentLicense, NumberOfEnrolledChildren, NumberOfProviderChildren,
        NumberOfParticipantsWithBloodTies, NumberOfParticipantsWithoutBloodTies, MinorsLiveWithProvider,
        RelationshipTypeId, OffersServiceToImmigrantChildren, HomeTypeId, AdministratorAuthorizedName,
        AdministratorBirthDate, OffersServiceToDifferentGroups, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, IsAuthorizedToOperate, HasFamilyDepartmentLicense, NumberOfEnrolledChildren, NumberOfProviderChildren,
        NumberOfParticipantsWithBloodTies, NumberOfParticipantsWithoutBloodTies, MinorsLiveWithProvider,
        RelationshipTypeId, OffersServiceToImmigrantChildren, HomeTypeId, AdministratorAuthorizedName,
        AdministratorBirthDate, OffersServiceToDifferentGroups, CreatedAt, UpdatedAt
    FROM SchoolDayCareHome;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolDayCareHome a SiteDayCareHome';
END

-- 6. Migrar datos de SchoolStaff a SiteStaff
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolStaff') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteStaff')
BEGIN
    INSERT INTO SiteStaff
        (
        SiteId, StaffId, AssignmentDate, AssignmentTypeId, IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, StaffId, AssignmentDate, AssignmentTypeId, IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt, UpdatedAt
    FROM SchoolStaff;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolStaff a SiteStaff';
END

-- 7. Migrar datos de SchoolParticipant a SiteParticipant
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolParticipant') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteParticipant')
BEGIN
    INSERT INTO SiteParticipant
        (
        SiteId, ParticipantTypeId, IsActive, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, ParticipantTypeId, IsActive, CreatedAt, UpdatedAt
    FROM SchoolParticipant;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolParticipant a SiteParticipant';
END

-- 8. Migrar datos de SchoolChildGroup a SiteChildGroup
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolChildGroup') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteChildGroup')
BEGIN
    INSERT INTO SiteChildGroup
        (
        SiteId, GroupName, NumberOfChildren, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, GroupName, NumberOfChildren, CreatedAt, UpdatedAt
    FROM SchoolChildGroup;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolChildGroup a SiteChildGroup';
END

-- 9. Migrar datos de SchoolOperatingDays a SiteOperatingDays
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolOperatingDays') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteOperatingDays')
BEGIN
    INSERT INTO SiteOperatingDays
        (
        SiteId, OperatingDate, StartTime, EndTime, Comment, IsActive, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, OperatingDate, StartTime, EndTime, Comment, IsActive, CreatedAt, UpdatedAt
    FROM SchoolOperatingDays;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolOperatingDays a SiteOperatingDays';
END

-- 10. Migrar datos de SchoolFacility a SiteFacility
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolFacility') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteFacility')
BEGIN
    INSERT INTO SiteFacility
        (
        SiteId, FacilityTypeId, Description, IsActive, CreatedAt, UpdatedAt
        )
    SELECT
        SchoolId, FacilityTypeId, Description, IsActive, CreatedAt, UpdatedAt
    FROM SchoolFacility;

    SET @RowsMigrated = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@RowsMigrated AS NVARCHAR(10)) + ' registros de SchoolFacility a SiteFacility';
END

-- Verificar integridad de los datos migrados
PRINT 'Verificando integridad de los datos migrados...';

-- Verificar que el número de registros coincida
DECLARE @SchoolCount INT, @SiteCount INT;

SELECT @SchoolCount = COUNT(*)
FROM School;
SELECT @SiteCount = COUNT(*)
FROM Site;

IF @SchoolCount = @SiteCount
BEGIN
    PRINT 'Verificación exitosa: ' + CAST(@SiteCount AS NVARCHAR(10)) + ' registros migrados correctamente';
END
ELSE
BEGIN
    SET @ErrorMessage = 'Error: Número de registros no coincide. School: ' + CAST(@SchoolCount AS NVARCHAR(10)) + ', Site: ' + CAST(@SiteCount AS NVARCHAR(10));
    PRINT @ErrorMessage;
    ROLLBACK TRANSACTION;
    RETURN;
END

PRINT 'Migración de datos completada exitosamente';
PRINT 'Los datos han sido migrados de las tablas School a las tablas Site';

COMMIT TRANSACTION;
