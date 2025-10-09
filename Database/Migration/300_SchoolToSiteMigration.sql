-- =============================================
-- Migration Script: 300_SchoolToSiteMigration
-- Descripción: Script principal de migración de School a Site
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- IMPORTANTE: Este script ejecuta todos los pasos de migración en orden
-- Ejecutar en un entorno de desarrollo primero para verificar

BEGIN TRANSACTION;

DECLARE @sql NVARCHAR(MAX);
DECLARE @errorMessage NVARCHAR(MAX);

PRINT '=============================================';
PRINT 'INICIANDO MIGRACIÓN DE SCHOOL A SITE';
PRINT '=============================================';

-- Paso 1: Crear nuevas tablas Site
PRINT 'Paso 1: Creando nuevas tablas Site...';

-- Crear tabla Site
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'Site')
BEGIN
    CREATE TABLE [dbo].[Site]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [AgencyId] [int] NOT NULL,
        [Name] [nvarchar](255) NOT NULL,
        [StartDate] [date] NULL,
        [Address] [nvarchar](255) NOT NULL,
        [CityId] [int] NOT NULL,
        [RegionId] [int] NOT NULL,
        [ZipCode] [nvarchar](20) NOT NULL,
        [Latitude] [float] NULL,
        [Longitude] [float] NULL,
        [PostalAddress] [nvarchar](255) NULL,
        [PostalCityId] [int] NULL,
        [PostalRegionId] [int] NULL,
        [PostalZipCode] [nvarchar](20) NULL,
        [SameAsPhysicalAddress] [bit] NULL,
        [OrganizationTypeId] [int] NOT NULL,
        [CenterTypeId] [int] NULL,
        [NonProfit] [bit] NULL,
        [BaseYear] [int] NULL,
        [RenewalYear] [int] NULL,
        [OperatingFromDate] [date] NULL,
        [OperatingToDate] [date] NULL,
        [OperatingDaysCalculated] [int] NULL,
        [KitchenTypeId] [int] NULL,
        [GroupTypeId] [int] NULL,
        [DeliveryTypeId] [int] NULL,
        [SponsorTypeId] [int] NULL,
        [ApplicantTypeId] [int] NULL,
        [ResidentialTypeId] [int] NULL,
        [OperatingPolicyId] [int] NULL,
        [AreaTypeId] [int] NULL,
        [LocationTypeId] [int] NULL,
        [HasWarehouse] [bit] NULL,
        [HasDiningRoom] [bit] NULL,
        [AdministratorAuthorizedName] [nvarchar](255) NULL,
        [SitePhone] [nvarchar](20) NULL,
        [Extension] [nvarchar](10) NULL,
        [MobilePhone] [nvarchar](20) NULL,
        [CommunityId] [int] NULL,
        [WalkersId] [int] NULL,
        [SiteTypeId] [int] NULL,
        [ExperienceId] [int] NULL,
        [ReviewResultId] [int] NULL,
        [ReviewDate] [datetime] NULL,
        [ReviewJustification] [nvarchar](500) NULL,
        [GeneralEnrollment] [int] NULL,
        [SiteNumber] [int] NOT NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [InactiveJustification] [nvarchar](500) NULL,
        [InactiveDate] [datetime] NULL,
        [SiteCode] [nvarchar](255) NULL,
        [SiteLocationId] [int] NULL,
        [ServiceTime] [datetime] NULL,
        [OrganizedAthleticPrograms] [bit] NULL,
        [AtRiskService] [bit] NULL,
        [IsMainSite] [bit] NOT NULL DEFAULT 0,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla Site creada';
END

-- Crear tabla SiteSatellite
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteSatellite')
BEGIN
    CREATE TABLE [dbo].[SiteSatellite]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [MainSiteId] [int] NOT NULL,
        [SatelliteSiteId] [int] NOT NULL,
        [AssignmentDate] [date] NULL,
        [Comment] [nvarchar](255) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteSatellite] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_SiteSatellite] UNIQUE ([SatelliteSiteId])
    );
    PRINT 'Tabla SiteSatellite creada';
END

-- Crear tabla SiteService
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteService')
BEGIN
    CREATE TABLE [dbo].[SiteService]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [ChildGroupId] [int] NULL,
        [Breakfast] [bit] NULL,
        [BreakfastFrom] [time] NULL,
        [BreakfastTo] [time] NULL,
        [Lunch] [bit] NULL,
        [LunchFrom] [time] NULL,
        [LunchTo] [time] NULL,
        [SnackAM] [bit] NULL,
        [SnackAMFrom] [time] NULL,
        [SnackAMTo] [time] NULL,
        [Dinner] [bit] NULL,
        [DinnerFrom] [time] NULL,
        [DinnerTo] [time] NULL,
        [SnackPM] [bit] NULL,
        [SnackPMFrom] [time] NULL,
        [SnackPMTo] [time] NULL,
        [SnackNight] [bit] NULL,
        [SnackNightFrom] [time] NULL,
        [SnackNightTo] [time] NULL,
        [DinnerExtended] [bit] NULL,
        [DinnerExtendedFrom] [time] NULL,
        [DinnerExtendedTo] [time] NULL,
        [DinnerAtRisk] [bit] NULL,
        [DinnerAtRiskFrom] [time] NULL,
        [DinnerAtRiskTo] [time] NULL,
        [SnackExtended] [bit] NULL,
        [SnackExtendedFrom] [time] NULL,
        [SnackExtendedTo] [time] NULL,
        [SnackAtRisk] [bit] NULL,
        [SnackAtRiskFrom] [time] NULL,
        [SnackAtRiskTo] [time] NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteService] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla SiteService creada';
END

-- Crear tabla SiteEducationLevel
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteEducationLevel')
BEGIN
    CREATE TABLE [dbo].[SiteEducationLevel]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [EducationLevelId] [int] NOT NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteEducationLevel] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_SiteEducationLevel_SiteId_EducationLevelId] UNIQUE ([SiteId], [EducationLevelId])
    );
    PRINT 'Tabla SiteEducationLevel creada';
END

-- Crear tabla SiteDayCareHome
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteDayCareHome')
BEGIN
    CREATE TABLE [dbo].[SiteDayCareHome]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [IsAuthorizedToOperate] [bit] NULL,
        [HasFamilyDepartmentLicense] [bit] NULL,
        [NumberOfEnrolledChildren] [int] NULL,
        [NumberOfProviderChildren] [int] NULL,
        [NumberOfParticipantsWithBloodTies] [int] NULL,
        [NumberOfParticipantsWithoutBloodTies] [int] NULL,
        [MinorsLiveWithProvider] [bit] NULL,
        [RelationshipTypeId] [int] NULL,
        [OffersServiceToImmigrantChildren] [bit] NULL,
        [HomeTypeId] [int] NULL,
        [AdministratorAuthorizedName] [nvarchar](255) NULL,
        [AdministratorBirthDate] [date] NULL,
        [OffersServiceToDifferentGroups] [bit] NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteDayCareHome] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_SiteDayCareHome_SiteId] UNIQUE ([SiteId])
    );
    PRINT 'Tabla SiteDayCareHome creada';
END

-- Crear tabla SiteStaff
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteStaff')
BEGIN
    CREATE TABLE [dbo].[SiteStaff]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [StaffId] [int] NOT NULL,
        [AssignmentDate] [date] NOT NULL DEFAULT GETDATE(),
        [AssignmentTypeId] [int] NOT NULL DEFAULT 1,
        [IsPrimary] [bit] NOT NULL DEFAULT 0,
        [StartDate] [date] NULL,
        [EndDate] [date] NULL,
        [Comments] [nvarchar](500) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteStaff] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla SiteStaff creada';
END

-- Crear tabla SiteParticipant
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteParticipant')
BEGIN
    CREATE TABLE [dbo].[SiteParticipant]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [ParticipantTypeId] [int] NOT NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteParticipant] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla SiteParticipant creada';
END

-- Crear tabla SiteChildGroup
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteChildGroup')
BEGIN
    CREATE TABLE [dbo].[SiteChildGroup]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [GroupName] [nvarchar](255) NOT NULL,
        [NumberOfChildren] [int] NOT NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteChildGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla SiteChildGroup creada';
END

-- Crear tabla SiteOperatingDays
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteOperatingDays')
BEGIN
    CREATE TABLE [dbo].[SiteOperatingDays]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [OperatingDate] [date] NOT NULL,
        [StartTime] [time] NULL,
        [EndTime] [time] NULL,
        [Comment] [nvarchar](255) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteOperatingDays] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla SiteOperatingDays creada';
END

-- Crear tabla SiteFacility
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteFacility')
BEGIN
    CREATE TABLE [dbo].[SiteFacility]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiteId] [int] NOT NULL,
        [FacilityTypeId] [int] NOT NULL,
        [Description] [nvarchar](255) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteFacility] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla SiteFacility creada';
END

-- Paso 2: Migrar datos de School a Site
PRINT 'Paso 2: Migrando datos de School a Site...';

IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'School') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'Site')
BEGIN
    -- Migrar datos de School a Site
    INSERT INTO Site
        (
        AgencyId, Name, StartDate, Address, CityId, RegionId, ZipCode, Latitude, Longitude,
        PostalAddress, PostalCityId, PostalRegionId, PostalZipCode, SameAsPhysicalAddress,
        OrganizationTypeId, CenterTypeId, NonProfit, BaseYear, RenewalYear, OperatingFromDate, OperatingToDate, OperatingDaysCalculated,
        KitchenTypeId, GroupTypeId, DeliveryTypeId, SponsorTypeId, ApplicantTypeId, ResidentialTypeId, OperatingPolicyId, AreaTypeId, LocationTypeId,
        HasWarehouse, HasDiningRoom, SitePhone, Extension, MobilePhone,
        CommunityId, WalkersId, SiteTypeId, ExperienceId, ReviewResultId, ReviewDate, ReviewJustification,
        SiteCode, SiteLocationId, GeneralEnrollment, SiteNumber, ServiceTime, OrganizedAthleticPrograms, AtRiskService,
        IsMainSite, IsActive, InactiveJustification, InactiveDate, CreatedAt, UpdatedAt
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

    PRINT 'Datos migrados de School a Site: ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' registros';
END

-- Migrar datos de tablas relacionadas
IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolSatellite') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteSatellite')
BEGIN
    INSERT INTO SiteSatellite
        (MainSiteId, SatelliteSiteId, AssignmentDate, Comment, IsActive, CreatedAt, UpdatedAt)
    SELECT MainSchoolId, SatelliteSchoolId, AssignmentDate, Comment, IsActive, CreatedAt, UpdatedAt
    FROM SchoolSatellite;
    PRINT 'Datos migrados de SchoolSatellite a SiteSatellite: ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' registros';
END

IF EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SchoolService') AND EXISTS (SELECT *
    FROM sys.tables
    WHERE name = 'SiteService')
BEGIN
    INSERT INTO SiteService
        (SiteId, ChildGroupId, Breakfast, BreakfastFrom, BreakfastTo, Lunch, LunchFrom, LunchTo,
        SnackAM, SnackAMFrom, SnackAMTo, Dinner, DinnerFrom, DinnerTo, SnackPM, SnackPMFrom, SnackPMTo,
        SnackNight, SnackNightFrom, SnackNightTo, DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo,
        DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo, SnackExtended, SnackExtendedFrom, SnackExtendedTo,
        SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo, CreatedAt, UpdatedAt)
    SELECT SchoolId, ChildGroupId, Breakfast, BreakfastFrom, BreakfastTo, Lunch, LunchFrom, LunchTo,
        SnackAM, SnackAMFrom, SnackAMTo, Dinner, DinnerFrom, DinnerTo, SnackPM, SnackPMFrom, SnackPMTo,
        SnackNight, SnackNightFrom, SnackNightTo, DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo,
        DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo, SnackExtended, SnackExtendedFrom, SnackExtendedTo,
        SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo, CreatedAt, UpdatedAt
    FROM SchoolService;
    PRINT 'Datos migrados de SchoolService a SiteService: ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' registros';
END

-- Paso 3: Crear stored procedures
PRINT 'Paso 3: Creando stored procedures...';

-- Crear 104_InsertSite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '104_InsertSite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[104_InsertSite]
        @agencyId INT,
        @name NVARCHAR(255),
        @startDate DATE = NULL,
        @address NVARCHAR(255),
        @cityId INT,
        @regionId INT,
        @zipCode NVARCHAR(20),
        @latitude FLOAT = NULL,
        @longitude FLOAT = NULL,
        @postalAddress NVARCHAR(255) = NULL,
        @postalCityId INT = NULL,
        @postalRegionId INT = NULL,
        @postalZipCode NVARCHAR(20) = NULL,
        @sameAsPhysicalAddress BIT = NULL,
        @organizationTypeId INT,
        @centerTypeId INT = NULL,
        @nonProfit BIT = NULL,
        @baseYear INT = NULL,
        @renewalYear INT = NULL,
        @operatingFromDate DATE = NULL,
        @operatingToDate DATE = NULL,
        @operatingDaysCalculated INT = NULL,
        @kitchenTypeId INT = NULL,
        @groupTypeId INT = NULL,
        @deliveryTypeId INT = NULL,
        @sponsorTypeId INT = NULL,
        @applicantTypeId INT = NULL,
        @residentialTypeId INT = NULL,
        @operatingPolicyId INT = NULL,
        @areaTypeId INT = NULL,
        @locationTypeId INT = NULL,
        @hasWarehouse BIT = NULL,
        @hasDiningRoom BIT = NULL,
        @administratorAuthorizedName NVARCHAR(255) = NULL,
        @sitePhone NVARCHAR(20) = NULL,
        @extension NVARCHAR(10) = NULL,
        @mobilePhone NVARCHAR(20) = NULL,
        @communityId INT = NULL,
        @walkersId INT = NULL,
        @siteTypeId INT = NULL,
        @experienceId INT = NULL,
        @reviewResultId INT = NULL,
        @reviewDate DATETIME = NULL,
        @reviewJustification NVARCHAR(500) = NULL,
        @siteCode NVARCHAR(20) = NULL,
        @generalEnrollment INT = NULL,
        @siteNumber INT,
        @serviceTime DATETIME = NULL,
        @organizedAthleticPrograms BIT = NULL,
        @atRiskService BIT = NULL,
        @id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @isMainSite BIT = 1;
        IF EXISTS (SELECT 1 FROM Site WHERE AgencyId = @agencyId AND IsMainSite = 1 AND IsActive = 1)
        BEGIN
            SET @isMainSite = 0;
        END
        INSERT INTO Site (
            AgencyId, Name, StartDate, Address, CityId, RegionId, ZipCode, Latitude, Longitude,
            PostalAddress, PostalCityId, PostalRegionId, PostalZipCode, SameAsPhysicalAddress,
            OrganizationTypeId, CenterTypeId, NonProfit, BaseYear, RenewalYear, OperatingFromDate, OperatingToDate, OperatingDaysCalculated,
            KitchenTypeId, GroupTypeId, DeliveryTypeId, SponsorTypeId, ApplicantTypeId, ResidentialTypeId, OperatingPolicyId, AreaTypeId, LocationTypeId,
            HasWarehouse, HasDiningRoom, AdministratorAuthorizedName, SitePhone, Extension, MobilePhone,
            CommunityId, WalkersId, SiteTypeId, ExperienceId, ReviewResultId, ReviewDate, ReviewJustification,
            SiteCode, GeneralEnrollment, SiteNumber, ServiceTime, OrganizedAthleticPrograms, AtRiskService,
            IsMainSite, IsActive, CreatedAt
        )
        VALUES (
            @agencyId, @name, @startDate, @address, @cityId, @regionId, @zipCode, @latitude, @longitude,
            @postalAddress, @postalCityId, @postalRegionId, @postalZipCode, @sameAsPhysicalAddress,
            @organizationTypeId, @centerTypeId, @nonProfit, @baseYear, @renewalYear, @operatingFromDate, @operatingToDate, @operatingDaysCalculated,
            @kitchenTypeId, @groupTypeId, @deliveryTypeId, @sponsorTypeId, @applicantTypeId, @residentialTypeId, @operatingPolicyId, @areaTypeId, @locationTypeId,
            @hasWarehouse, @hasDiningRoom, @administratorAuthorizedName, @sitePhone, @extension, @mobilePhone,
            @communityId, @walkersId, @siteTypeId, @experienceId, @reviewResultId, @reviewDate, @reviewJustification,
            @siteCode, @generalEnrollment, @siteNumber, @serviceTime, @organizedAthleticPrograms, @atRiskService,
            @isMainSite, 1, GETDATE()
        );
        SET @id = SCOPE_IDENTITY();
    END
    ');
    PRINT 'Stored procedure 104_InsertSite creado';
END

-- Crear 105_GetSiteById
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '105_GetSiteById')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[105_GetSiteById]
        @id INT
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT
            s.Id, s.AgencyId, a.Name AS AgencyName, s.Name, s.StartDate, s.Address,
            s.CityId, c.Name AS CityName, s.RegionId, r.Name AS RegionName, s.ZipCode,
            s.Latitude, s.Longitude, s.PostalAddress, s.PostalCityId, c2.Name AS PostalCityName,
            s.PostalRegionId, r2.Name AS PostalRegionName, s.PostalZipCode, s.SameAsPhysicalAddress,
            s.OrganizationTypeId, ot.Name AS OrganizationTypeName, ot.NameEN AS OrganizationTypeNameEN,
            s.CenterTypeId, ct.Name AS CenterName, ct.NameEN AS CenterNameEN, s.NonProfit,
            s.BaseYear, s.RenewalYear, s.OperatingFromDate, s.OperatingToDate, s.OperatingDaysCalculated,
            s.ServiceTime, s.KitchenTypeId, kt.Name AS KitchenTypeName, kt.NameEN AS KitchenTypeNameEN,
            s.GroupTypeId, gt.Name AS GroupTypeName, gt.NameEN AS GroupTypeNameEN, s.DeliveryTypeId,
            dt.Name AS DeliveryTypeName, dt.NameEN AS DeliveryTypeNameEN, s.SponsorTypeId,
            st.Name AS SponsorTypeName, st.NameEN AS SponsorTypeNameEN, s.ApplicantTypeId,
            at.Name AS ApplicantTypeName, at.NameEN AS ApplicantTypeNameEN, s.ResidentialTypeId,
            rt.Name AS ResidentialTypeName, rt.NameEN AS ResidentialTypeNameEN, s.OperatingPolicyId,
            opol.Name AS OperatingPolicyName, opol.NameEN AS OperatingPolicyNameEN, s.AreaTypeId,
            atype.Name AS AreaTypeName, atype.NameEN AS AreaTypeNameEN, s.LocationTypeId,
            ltype.Name AS LocationTypeName, ltype.NameEN AS LocationTypeNameEN, s.HasWarehouse,
            s.HasDiningRoom, s.SitePhone, s.Extension, s.MobilePhone, s.CommunityId, s.WalkersId,
            s.SiteTypeId, s.ExperienceId, s.ReviewResultId, s.ReviewDate, s.ReviewJustification,
            s.IsActive, s.InactiveJustification, s.InactiveDate, s.GeneralEnrollment, s.SiteNumber,
            s.OrganizedAthleticPrograms, s.AtRiskService, s.IsMainSite, a.AgencyCode,
            CASE 
                WHEN a.AgencyCode IS NOT NULL AND s.SiteNumber IS NOT NULL THEN
                    SUBSTRING(a.AgencyCode, CHARINDEX(''-'', a.AgencyCode) + 1, LEN(a.AgencyCode)) + ''-'' + CAST(s.SiteNumber AS VARCHAR(10))
                ELSE NULL
            END AS SiteCode,
            s.CreatedAt, s.UpdatedAt
        FROM Site s
            LEFT JOIN Agency a ON s.AgencyId = a.Id
            LEFT JOIN City c ON s.CityId = c.Id
            LEFT JOIN Region r ON s.RegionId = r.Id
            LEFT JOIN City c2 ON s.PostalCityId = c2.Id
            LEFT JOIN Region r2 ON s.PostalRegionId = r2.Id
            LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
            LEFT JOIN CenterType ct ON s.CenterTypeId = ct.Id
            LEFT JOIN KitchenType kt ON s.KitchenTypeId = kt.Id
            LEFT JOIN GroupType gt ON s.GroupTypeId = gt.Id
            LEFT JOIN DeliveryType dt ON s.DeliveryTypeId = dt.Id
            LEFT JOIN SponsorType st ON s.SponsorTypeId = st.Id
            LEFT JOIN OptionSelection at ON s.ApplicantTypeId = at.Id
            LEFT JOIN OptionSelection rt ON s.ResidentialTypeId = rt.Id
            LEFT JOIN OperatingPolicy opol ON s.OperatingPolicyId = opol.Id
            LEFT JOIN AreaType atype ON s.AreaTypeId = atype.Id
            LEFT JOIN AreaType ltype ON s.LocationTypeId = ltype.Id
        WHERE s.Id = @id;
    END
    ');
    PRINT 'Stored procedure 105_GetSiteById creado';
END

-- Crear 104_GetSites
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '104_GetSites')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[104_GetSites]
        @take INT,
        @skip INT,
        @name NVARCHAR(255) = NULL,
        @cityId INT = NULL,
        @regionId INT = NULL,
        @agencyId INT = NULL,
        @alls BIT = 0
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT
            s.Id, s.Name, s.Address, c.Name AS CityName, r.Name AS RegionName,
            s.IsMainSite, s2.Name AS MainSiteName, s.GeneralEnrollment, s.SiteNumber,
            a.AgencyCode,
            CASE 
                WHEN a.AgencyCode IS NOT NULL AND s.SiteNumber IS NOT NULL THEN
                    SUBSTRING(a.AgencyCode, CHARINDEX(''-'', a.AgencyCode) + 1, LEN(a.AgencyCode)) + ''-'' + CAST(s.SiteNumber AS VARCHAR(10))
                ELSE NULL
            END AS SiteCode
        FROM Site s
            INNER JOIN City c ON s.CityId = c.Id
            INNER JOIN Region r ON s.RegionId = r.Id
            LEFT JOIN Agency a ON s.AgencyId = a.Id
            LEFT JOIN SiteSatellite ss ON s.Id = ss.SatelliteSiteId
            LEFT JOIN Site s2 ON ss.MainSiteId = s2.Id
        WHERE s.IsActive = 1
            AND (
                @alls = 1
            OR ((@name IS NULL OR s.Name LIKE ''%'' + @name + ''%'')
            AND (@cityId IS NULL OR s.CityId = @cityId)
            AND (@regionId IS NULL OR s.RegionId = @regionId)
            AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
                )
            )
        ORDER BY s.IsMainSite DESC, s.Name
        OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
        SELECT COUNT(*)
        FROM Site s
        WHERE s.IsActive = 1
            AND (
                @alls = 1
            OR ((@name IS NULL OR s.Name LIKE ''%'' + @name + ''%'')
            AND (@cityId IS NULL OR s.CityId = @cityId)
            AND (@regionId IS NULL OR s.RegionId = @regionId)
            AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
                )
            );
    END
    ');
    PRINT 'Stored procedure 104_GetSites creado';
END

-- Crear 102_DeleteSite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '102_DeleteSite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[102_DeleteSite]
        @Id INT
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE Site 
        SET 
            IsActive = 0,
            InactiveDate = GETDATE(),
            UpdatedAt = GETDATE()
        WHERE Id = @Id;
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 102_DeleteSite creado';
END

-- Crear 104_UpdateSite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '104_UpdateSite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[104_UpdateSite]
        @id INT,
        @agencyId INT,
        @name NVARCHAR(255),
        @startDate DATE = NULL,
        @address NVARCHAR(255),
        @cityId INT,
        @regionId INT,
        @zipCode NVARCHAR(20),
        @latitude FLOAT = NULL,
        @longitude FLOAT = NULL,
        @postalAddress NVARCHAR(255) = NULL,
        @postalCityId INT = NULL,
        @postalRegionId INT = NULL,
        @postalZipCode NVARCHAR(20) = NULL,
        @sameAsPhysicalAddress BIT = NULL,
        @organizationTypeId INT,
        @centerTypeId INT = NULL,
        @areaTypeId INT = NULL,
        @locationTypeId INT = NULL,
        @nonProfit BIT = NULL,
        @baseYear INT = NULL,
        @renewalYear INT = NULL,
        @operatingFromDate DATE = NULL,
        @operatingToDate DATE = NULL,
        @operatingDaysCalculated INT = NULL,
        @kitchenTypeId INT = NULL,
        @groupTypeId INT = NULL,
        @deliveryTypeId INT = NULL,
        @sponsorTypeId INT = NULL,
        @applicantTypeId INT = NULL,
        @residentialTypeId INT = NULL,
        @operatingPolicyId INT = NULL,
        @hasWarehouse BIT = NULL,
        @hasDiningRoom BIT = NULL,
        @sitePhone NVARCHAR(20) = NULL,
        @extension NVARCHAR(10) = NULL,
        @mobilePhone NVARCHAR(20) = NULL,
        @communityId INT = NULL,
        @walkersId INT = NULL,
        @siteTypeId INT = NULL,
        @experienceId INT = NULL,
        @reviewResultId INT = NULL,
        @reviewDate DATETIME = NULL,
        @reviewJustification NVARCHAR(500) = NULL,
        @isActive BIT = NULL,
        @inactiveJustification NVARCHAR(500) = NULL,
        @inactiveDate DATETIME = NULL,
        @generalEnrollment INT = NULL,
        @siteNumber INT,
        @serviceTime DATETIME = NULL,
        @organizedAthleticPrograms BIT = NULL,
        @atRiskService BIT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE Site 
        SET 
            AgencyId = @agencyId,
            Name = @name,
            StartDate = @startDate,
            Address = @address,
            CityId = @cityId,
            RegionId = @regionId,
            ZipCode = @zipCode,
            Latitude = @latitude,
            Longitude = @longitude,
            PostalAddress = @postalAddress,
            PostalCityId = @postalCityId,
            PostalRegionId = @postalRegionId,
            PostalZipCode = @postalZipCode,
            SameAsPhysicalAddress = @sameAsPhysicalAddress,
            OrganizationTypeId = @organizationTypeId,
            CenterTypeId = @centerTypeId,
            AreaTypeId = @areaTypeId,
            LocationTypeId = @locationTypeId,
            NonProfit = @nonProfit,
            BaseYear = @baseYear,
            RenewalYear = @renewalYear,
            OperatingFromDate = @operatingFromDate,
            OperatingToDate = @operatingToDate,
            OperatingDaysCalculated = @operatingDaysCalculated,
            KitchenTypeId = @kitchenTypeId,
            GroupTypeId = @groupTypeId,
            DeliveryTypeId = @deliveryTypeId,
            SponsorTypeId = @sponsorTypeId,
            ApplicantTypeId = @applicantTypeId,
            ResidentialTypeId = @residentialTypeId,
            OperatingPolicyId = @operatingPolicyId,
            HasWarehouse = @hasWarehouse,
            HasDiningRoom = @hasDiningRoom,
            SitePhone = @sitePhone,
            Extension = @extension,
            MobilePhone = @mobilePhone,
            CommunityId = @communityId,
            WalkersId = @walkersId,
            SiteTypeId = @siteTypeId,
            ExperienceId = @experienceId,
            ReviewResultId = @reviewResultId,
            ReviewDate = @reviewDate,
            ReviewJustification = @reviewJustification,
            IsActive = @isActive,
            InactiveJustification = @inactiveJustification,
            InactiveDate = @inactiveDate,
            GeneralEnrollment = @generalEnrollment,
            SiteNumber = @siteNumber,
            ServiceTime = @serviceTime,
            OrganizedAthleticPrograms = @organizedAthleticPrograms,
            AtRiskService = @atRiskService,
            UpdatedAt = GETDATE()
        WHERE Id = @id;
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 104_UpdateSite creado';
END

-- Crear 102_HasMainSite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '102_HasMainSite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[102_HasMainSite]
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT COUNT(*) AS HasMainSite
        FROM Site 
        WHERE IsMainSite = 1 AND IsActive = 1;
    END
    ');
    PRINT 'Stored procedure 102_HasMainSite creado';
END

-- Crear 100_InsertSiteOperatingDays
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteOperatingDays')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteOperatingDays]
        @siteId INT,
        @operatingFromDate DATE,
        @operatingToDate DATE,
        @defaultStartTime TIME = ''08:00:00'',
        @defaultEndTime TIME = ''16:00:00'',
        @defaultComment NVARCHAR(500) = ''Día de funcionamiento generado automáticamente''
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @CurrentDate DATE = @operatingFromDate;
        DECLARE @RowsInserted INT = 0;
        WHILE @CurrentDate <= @operatingToDate
        BEGIN
            INSERT INTO SiteOperatingDays (SiteId, OperatingDate, StartTime, EndTime, Comment, IsActive, CreatedAt)
            VALUES (@siteId, @CurrentDate, @defaultStartTime, @defaultEndTime, @defaultComment, 1, GETDATE());
            SET @RowsInserted = @RowsInserted + 1;
            SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
        END
        SELECT @RowsInserted AS DaysInserted;
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteOperatingDays creado';
END

-- Crear 100_GetSiteOperatingDays
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_GetSiteOperatingDays')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_GetSiteOperatingDays]
        @siteId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT s.Id as SiteId, s.Name as SiteName FROM Site s WHERE s.Id = @siteId;
        SELECT sod.Id, sod.SiteId, sod.OperatingDate, sod.StartTime, sod.EndTime, sod.Comment, sod.IsActive, sod.CreatedAt, sod.UpdatedAt
        FROM SiteOperatingDays sod WHERE sod.SiteId = @siteId ORDER BY sod.OperatingDate;
    END
    ');
    PRINT 'Stored procedure 100_GetSiteOperatingDays creado';
END

-- Crear 100_ToggleSiteOperatingDay
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_ToggleSiteOperatingDay')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_ToggleSiteOperatingDay]
        @siteId INT,
        @operatingDate DATE,
        @startTime TIME = NULL,
        @endTime TIME = NULL,
        @comment NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @ExistingId INT;
        SELECT @ExistingId = Id FROM SiteOperatingDays WHERE SiteId = @siteId AND OperatingDate = @operatingDate;
        IF @ExistingId IS NOT NULL
        BEGIN
            UPDATE SiteOperatingDays SET StartTime = @startTime, EndTime = @endTime, Comment = @comment, UpdatedAt = GETDATE() WHERE Id = @ExistingId;
        END
        ELSE
        BEGIN
            INSERT INTO SiteOperatingDays (SiteId, OperatingDate, StartTime, EndTime, Comment, IsActive, CreatedAt)
            VALUES (@siteId, @operatingDate, @startTime, @endTime, @comment, 1, GETDATE());
        END
        SELECT @@ROWCOUNT as RowsAffected;
    END
    ');
    PRINT 'Stored procedure 100_ToggleSiteOperatingDay creado';
END

-- Crear 102_UpdateSiteFacilityIsActive
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '102_UpdateSiteFacilityIsActive')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[102_UpdateSiteFacilityIsActive]
        @siteId INT,
        @isActive BIT
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE SiteFacility SET IsActive = @isActive WHERE SiteId = @siteId;
    END
    ');
    PRINT 'Stored procedure 102_UpdateSiteFacilityIsActive creado';
END

-- Crear 100_GetNextSiteNumber
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_GetNextSiteNumber')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_GetNextSiteNumber]
        @agencyId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT ISNULL(MAX(SiteNumber), 0) + 1 AS NextSiteNumber FROM Site WHERE AgencyId = @agencyId;
    END
    ');
    PRINT 'Stored procedure 100_GetNextSiteNumber creado';
END

-- Crear 103_UpdateSiteActiveStatus
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '103_UpdateSiteActiveStatus')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[103_UpdateSiteActiveStatus]
        @id INT,
        @isActive BIT,
        @inactiveJustification NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE Site SET IsActive = @isActive, InactiveJustification = @inactiveJustification, InactiveDate = CASE WHEN @isActive = 0 THEN GETDATE() ELSE NULL END, UpdatedAt = GETDATE() WHERE Id = @id;
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 103_UpdateSiteActiveStatus creado';
END

-- Paso 4: Verificar migración
PRINT 'Paso 4: Verificando migración...';

DECLARE @schoolCount INT, @siteCount INT;
SELECT @schoolCount = COUNT(*)
FROM School;
SELECT @siteCount = COUNT(*)
FROM Site;

IF @schoolCount = @siteCount
BEGIN
    PRINT 'Verificación exitosa: ' + CAST(@siteCount AS NVARCHAR(10)) + ' registros migrados correctamente';
END
ELSE
BEGIN
    SET @errorMessage = 'Error: Número de registros no coincide. School: ' + CAST(@schoolCount AS NVARCHAR(10)) + ', Site: ' + CAST(@siteCount AS NVARCHAR(10));
    PRINT @errorMessage;
    ROLLBACK TRANSACTION;
    RETURN;
END

PRINT '=============================================';
PRINT 'MIGRACIÓN COMPLETADA EXITOSAMENTE';
PRINT '=============================================';

COMMIT TRANSACTION;