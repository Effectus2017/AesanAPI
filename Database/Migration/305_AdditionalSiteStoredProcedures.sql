-- =============================================
-- Migration Script: 305_AdditionalSiteStoredProcedures
-- Descripción: Stored procedures adicionales para Site que no estaban en la migración principal
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- IMPORTANTE: Este script debe ejecutarse DESPUÉS de 300_SchoolToSiteMigration.sql
-- Contiene stored procedures adicionales que faltaban en la migración principal

BEGIN TRANSACTION;

PRINT '=============================================';
PRINT 'CREANDO STORED PROCEDURES ADICIONALES PARA SITE';
PRINT '=============================================';

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

-- Crear stored procedures para SiteSatellite
-- Crear 102_InsertSatelliteSite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '102_InsertSatelliteSite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[102_InsertSatelliteSite]
        @mainSiteId INT,
        @satelliteSiteId INT,
        @comment NVARCHAR(255) = NULL,
        @isActive BIT = 1,
        @id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        INSERT INTO SiteSatellite (MainSiteId, SatelliteSiteId, AssignmentDate, Comment, IsActive, CreatedAt)
        VALUES (@mainSiteId, @satelliteSiteId, GETDATE(), @comment, @isActive, GETDATE());
        SET @id = SCOPE_IDENTITY();
    END
    ');
    PRINT 'Stored procedure 102_InsertSatelliteSite creado';
END

-- Crear 103_UpdateSiteSatellite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '103_UpdateSiteSatellite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[103_UpdateSiteSatellite]
        @mainSiteId INT,
        @satelliteSiteId INT,
        @comment NVARCHAR(255) = NULL,
        @isActive BIT = 1
    AS
    BEGIN
        SET NOCOUNT ON;
        IF EXISTS (SELECT 1 FROM SiteSatellite WHERE SatelliteSiteId = @satelliteSiteId)
        BEGIN
            UPDATE SiteSatellite SET MainSiteId = @mainSiteId, Comment = @comment, IsActive = @isActive, UpdatedAt = GETDATE() WHERE SatelliteSiteId = @satelliteSiteId;
        END
        ELSE
        BEGIN
            INSERT INTO SiteSatellite (MainSiteId, SatelliteSiteId, AssignmentDate, Comment, IsActive, CreatedAt)
            VALUES (@mainSiteId, @satelliteSiteId, GETDATE(), @comment, @isActive, GETDATE());
        END
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 103_UpdateSiteSatellite creado';
END

-- Crear stored procedures para SiteService
-- Crear 100_InsertSiteService
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteService')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteService]
        @siteId INT,
        @childGroupId INT = NULL,
        @breakfast BIT = NULL,
        @breakfastFrom TIME = NULL,
        @breakfastTo TIME = NULL,
        @lunch BIT = NULL,
        @lunchFrom TIME = NULL,
        @lunchTo TIME = NULL,
        @snackAM BIT = NULL,
        @snackAMFrom TIME = NULL,
        @snackAMTo TIME = NULL,
        @dinner BIT = NULL,
        @dinnerFrom TIME = NULL,
        @dinnerTo TIME = NULL,
        @snackPM BIT = NULL,
        @snackPMFrom TIME = NULL,
        @snackPMTo TIME = NULL,
        @snackNight BIT = NULL,
        @snackNightFrom TIME = NULL,
        @snackNightTo TIME = NULL,
        @dinnerExtended BIT = NULL,
        @dinnerExtendedFrom TIME = NULL,
        @dinnerExtendedTo TIME = NULL,
        @dinnerAtRisk BIT = NULL,
        @dinnerAtRiskFrom TIME = NULL,
        @dinnerAtRiskTo TIME = NULL,
        @snackExtended BIT = NULL,
        @snackExtendedFrom TIME = NULL,
        @snackExtendedTo TIME = NULL,
        @snackAtRisk BIT = NULL,
        @snackAtRiskFrom TIME = NULL,
        @snackAtRiskTo TIME = NULL,
        @id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        INSERT INTO SiteService (SiteId, ChildGroupId, Breakfast, BreakfastFrom, BreakfastTo, Lunch, LunchFrom, LunchTo,
            SnackAM, SnackAMFrom, SnackAMTo, Dinner, DinnerFrom, DinnerTo, SnackPM, SnackPMFrom, SnackPMTo,
            SnackNight, SnackNightFrom, SnackNightTo, DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo,
            DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo, SnackExtended, SnackExtendedFrom, SnackExtendedTo,
            SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo, CreatedAt)
        VALUES (@siteId, @childGroupId, @breakfast, @breakfastFrom, @breakfastTo, @lunch, @lunchFrom, @lunchTo,
            @snackAM, @snackAMFrom, @snackAMTo, @dinner, @dinnerFrom, @dinnerTo, @snackPM, @snackPMFrom, @snackPMTo,
            @snackNight, @snackNightFrom, @snackNightTo, @dinnerExtended, @dinnerExtendedFrom, @dinnerExtendedTo,
            @dinnerAtRisk, @dinnerAtRiskFrom, @dinnerAtRiskTo, @snackExtended, @snackExtendedFrom, @snackExtendedTo,
            @snackAtRisk, @snackAtRiskFrom, @snackAtRiskTo, GETDATE());
        SET @id = SCOPE_IDENTITY();
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteService creado';
END

-- Crear stored procedures para SiteEducationLevel
-- Crear 100_InsertSiteEducationLevels
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteEducationLevels')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteEducationLevels]
        @siteId INT,
        @educationLevelIds NVARCHAR(MAX)
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @EducationLevelTable TABLE (EducationLevelId INT);
        INSERT INTO @EducationLevelTable (EducationLevelId)
        SELECT CAST(value AS INT) FROM STRING_SPLIT(@educationLevelIds, '','') WHERE value IS NOT NULL AND value != '''';
        INSERT INTO SiteEducationLevel (SiteId, EducationLevelId, IsActive, CreatedAt)
        SELECT @siteId, EducationLevelId, 1, GETDATE() FROM @EducationLevelTable;
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteEducationLevels creado';
END

-- Crear 100_UpdateSiteEducationLevels
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_UpdateSiteEducationLevels')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_UpdateSiteEducationLevels]
        @siteId INT,
        @educationLevelIds NVARCHAR(MAX)
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @EducationLevelTable TABLE (EducationLevelId INT);
        INSERT INTO @EducationLevelTable (EducationLevelId)
        SELECT CAST(value AS INT) FROM STRING_SPLIT(@educationLevelIds, '','') WHERE value IS NOT NULL AND value != '''';
        DELETE FROM SiteEducationLevel WHERE SiteId = @siteId;
        INSERT INTO SiteEducationLevel (SiteId, EducationLevelId, IsActive, CreatedAt)
        SELECT @siteId, EducationLevelId, 1, GETDATE() FROM @EducationLevelTable;
    END
    ');
    PRINT 'Stored procedure 100_UpdateSiteEducationLevels creado';
END

-- Crear stored procedures para SiteDayCareHome
-- Crear 100_InsertSiteDayCareHome
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteDayCareHome')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteDayCareHome]
        @siteId INT,
        @isAuthorizedToOperate BIT = NULL,
        @hasFamilyDepartmentLicense BIT = NULL,
        @numberOfEnrolledChildren INT = NULL,
        @numberOfProviderChildren INT = NULL,
        @numberOfParticipantsWithBloodTies INT = NULL,
        @numberOfParticipantsWithoutBloodTies INT = NULL,
        @minorsLiveWithProvider BIT = NULL,
        @relationshipTypeId INT = NULL,
        @offersServiceToImmigrantChildren BIT = NULL,
        @homeTypeId INT = NULL,
        @administratorAuthorizedName NVARCHAR(255) = NULL,
        @administratorBirthDate DATE = NULL,
        @offersServiceToDifferentGroups BIT = NULL,
        @id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        INSERT INTO SiteDayCareHome (SiteId, IsAuthorizedToOperate, HasFamilyDepartmentLicense, NumberOfEnrolledChildren, NumberOfProviderChildren,
            NumberOfParticipantsWithBloodTies, NumberOfParticipantsWithoutBloodTies, MinorsLiveWithProvider,
            RelationshipTypeId, OffersServiceToImmigrantChildren, HomeTypeId, AdministratorAuthorizedName,
            AdministratorBirthDate, OffersServiceToDifferentGroups, CreatedAt)
        VALUES (@siteId, @isAuthorizedToOperate, @hasFamilyDepartmentLicense, @numberOfEnrolledChildren, @numberOfProviderChildren,
            @numberOfParticipantsWithBloodTies, @numberOfParticipantsWithoutBloodTies, @minorsLiveWithProvider,
            @relationshipTypeId, @offersServiceToImmigrantChildren, @homeTypeId, @administratorAuthorizedName,
            @administratorBirthDate, @offersServiceToDifferentGroups, GETDATE());
        SET @id = SCOPE_IDENTITY();
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteDayCareHome creado';
END

-- Crear 100_UpdateSiteDayCareHome
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_UpdateSiteDayCareHome')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_UpdateSiteDayCareHome]
        @siteId INT,
        @isAuthorizedToOperate BIT = NULL,
        @hasFamilyDepartmentLicense BIT = NULL,
        @numberOfEnrolledChildren INT = NULL,
        @numberOfProviderChildren INT = NULL,
        @numberOfParticipantsWithBloodTies INT = NULL,
        @numberOfParticipantsWithoutBloodTies INT = NULL,
        @minorsLiveWithProvider BIT = NULL,
        @relationshipTypeId INT = NULL,
        @offersServiceToImmigrantChildren BIT = NULL,
        @homeTypeId INT = NULL,
        @administratorAuthorizedName NVARCHAR(255) = NULL,
        @administratorBirthDate DATE = NULL,
        @offersServiceToDifferentGroups BIT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE SiteDayCareHome SET IsAuthorizedToOperate = @isAuthorizedToOperate, HasFamilyDepartmentLicense = @hasFamilyDepartmentLicense,
            NumberOfEnrolledChildren = @numberOfEnrolledChildren, NumberOfProviderChildren = @numberOfProviderChildren,
            NumberOfParticipantsWithBloodTies = @numberOfParticipantsWithBloodTies, NumberOfParticipantsWithoutBloodTies = @numberOfParticipantsWithoutBloodTies,
            MinorsLiveWithProvider = @minorsLiveWithProvider, RelationshipTypeId = @relationshipTypeId, OffersServiceToImmigrantChildren = @offersServiceToImmigrantChildren,
            HomeTypeId = @homeTypeId, AdministratorAuthorizedName = @administratorAuthorizedName, AdministratorBirthDate = @administratorBirthDate,
            OffersServiceToDifferentGroups = @offersServiceToDifferentGroups, UpdatedAt = GETDATE() WHERE SiteId = @siteId;
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 100_UpdateSiteDayCareHome creado';
END

-- Crear stored procedures para SiteParticipant
-- Crear 100_InsertSiteParticipants
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteParticipants')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteParticipants]
        @siteId INT,
        @participantTypeIds NVARCHAR(MAX)
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @ParticipantTypeTable TABLE (ParticipantTypeId INT);
        INSERT INTO @ParticipantTypeTable (ParticipantTypeId)
        SELECT CAST(value AS INT) FROM STRING_SPLIT(@participantTypeIds, '','') WHERE value IS NOT NULL AND value != '''';
        INSERT INTO SiteParticipant (SiteId, ParticipantTypeId, IsActive, CreatedAt)
        SELECT @siteId, ParticipantTypeId, 1, GETDATE() FROM @ParticipantTypeTable;
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteParticipants creado';
END

-- Crear 100_UpdateSiteParticipants
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_UpdateSiteParticipants')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_UpdateSiteParticipants]
        @siteId INT,
        @participantTypeIds NVARCHAR(MAX)
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @ParticipantTypeTable TABLE (ParticipantTypeId INT);
        INSERT INTO @ParticipantTypeTable (ParticipantTypeId)
        SELECT CAST(value AS INT) FROM STRING_SPLIT(@participantTypeIds, '','') WHERE value IS NOT NULL AND value != '''';
        DELETE FROM SiteParticipant WHERE SiteId = @siteId;
        INSERT INTO SiteParticipant (SiteId, ParticipantTypeId, IsActive, CreatedAt)
        SELECT @siteId, ParticipantTypeId, 1, GETDATE() FROM @ParticipantTypeTable;
    END
    ');
    PRINT 'Stored procedure 100_UpdateSiteParticipants creado';
END

-- Crear stored procedures para SiteChildGroup
-- Crear 100_InsertSiteChildGroup
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteChildGroup')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteChildGroup]
        @siteId INT,
        @groupName NVARCHAR(255),
        @numberOfChildren INT,
        @id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        INSERT INTO SiteChildGroup (SiteId, GroupName, NumberOfChildren, CreatedAt)
        VALUES (@siteId, @groupName, @numberOfChildren, GETDATE());
        SET @id = SCOPE_IDENTITY();
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteChildGroup creado';
END

-- Crear 100_DeleteSiteChildGroupsBySiteId
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_DeleteSiteChildGroupsBySiteId')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_DeleteSiteChildGroupsBySiteId]
        @siteId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM SiteChildGroup WHERE SiteId = @siteId;
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 100_DeleteSiteChildGroupsBySiteId creado';
END

-- Crear stored procedures para SiteStaff
-- Crear 100_InsertSiteStaff
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteStaff')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteStaff]
        @siteId INT,
        @staffId INT,
        @assignmentTypeId INT = 1,
        @isPrimary BIT = 0,
        @startDate DATE = NULL,
        @endDate DATE = NULL,
        @comments NVARCHAR(500) = NULL,
        @isActive BIT = 1,
        @id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        INSERT INTO SiteStaff (SiteId, StaffId, AssignmentDate, AssignmentTypeId, IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt)
        VALUES (@siteId, @staffId, GETDATE(), @assignmentTypeId, @isPrimary, @startDate, @endDate, @comments, @isActive, GETDATE());
        SET @id = SCOPE_IDENTITY();
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteStaff creado';
END

-- Crear 100_GetStaffBySite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_GetStaffBySite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_GetStaffBySite]
        @siteId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT ss.Id, ss.SiteId, ss.StaffId, s.FirstName, s.LastName, s.Email, s.Phone, ss.AssignmentDate,
            ss.AssignmentTypeId, at.Name AS AssignmentTypeName, at.NameEN AS AssignmentTypeNameEN, ss.IsPrimary,
            ss.StartDate, ss.EndDate, ss.Comments, ss.IsActive, ss.CreatedAt, ss.UpdatedAt
        FROM SiteStaff ss INNER JOIN Staff s ON ss.StaffId = s.Id
        LEFT JOIN OptionSelection at ON ss.AssignmentTypeId = at.Id
        WHERE ss.SiteId = @siteId AND ss.IsActive = 1 ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
    END
    ');
    PRINT 'Stored procedure 100_GetStaffBySite creado';
END

-- Crear stored procedures para SiteFacility
-- Crear 100_InsertSiteFacilities
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_InsertSiteFacilities')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_InsertSiteFacilities]
        @siteId INT,
        @facilityTypeIds NVARCHAR(MAX)
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @FacilityTypeTable TABLE (FacilityTypeId INT);
        INSERT INTO @FacilityTypeTable (FacilityTypeId)
        SELECT CAST(value AS INT) FROM STRING_SPLIT(@facilityTypeIds, '','') WHERE value IS NOT NULL AND value != '''';
        INSERT INTO SiteFacility (SiteId, FacilityTypeId, IsActive, CreatedAt)
        SELECT @siteId, FacilityTypeId, 1, GETDATE() FROM @FacilityTypeTable;
    END
    ');
    PRINT 'Stored procedure 100_InsertSiteFacilities creado';
END

-- Crear 100_UpdateSiteFacilities
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_UpdateSiteFacilities')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_UpdateSiteFacilities]
        @siteId INT,
        @facilityTypeIds NVARCHAR(MAX)
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @FacilityTypeTable TABLE (FacilityTypeId INT);
        INSERT INTO @FacilityTypeTable (FacilityTypeId)
        SELECT CAST(value AS INT) FROM STRING_SPLIT(@facilityTypeIds, '','') WHERE value IS NOT NULL AND value != '''';
        DELETE FROM SiteFacility WHERE SiteId = @siteId;
        INSERT INTO SiteFacility (SiteId, FacilityTypeId, IsActive, CreatedAt)
        SELECT @siteId, FacilityTypeId, 1, GETDATE() FROM @FacilityTypeTable;
    END
    ');
    PRINT 'Stored procedure 100_UpdateSiteFacilities creado';
END

-- Crear 100_GetFacilitiesBySite
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_GetFacilitiesBySite')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_GetFacilitiesBySite]
        @siteId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT sf.Id, sf.SiteId, sf.FacilityTypeId, ft.Name AS FacilityTypeName, ft.NameEN AS FacilityTypeNameEN,
            ft.OptionKey AS FacilityTypeOptionKey, sf.Description, sf.IsActive, sf.CreatedAt, sf.UpdatedAt
        FROM SiteFacility sf INNER JOIN OptionSelection ft ON sf.FacilityTypeId = ft.Id
        WHERE sf.SiteId = @siteId AND sf.IsActive = 1 ORDER BY ft.Name;
    END
    ');
    PRINT 'Stored procedure 100_GetFacilitiesBySite creado';
END

-- Crear 100_DeleteSiteFacilitiesBySiteId
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_DeleteSiteFacilitiesBySiteId')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_DeleteSiteFacilitiesBySiteId]
        @siteId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM SiteFacility WHERE SiteId = @siteId;
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 100_DeleteSiteFacilitiesBySiteId creado';
END

-- Crear stored procedures para SiteOperatingDays
-- Crear 100_DeleteSiteOperatingDaysBySiteId
IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = '100_DeleteSiteOperatingDaysBySiteId')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[100_DeleteSiteOperatingDaysBySiteId]
        @siteId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM SiteOperatingDays WHERE SiteId = @siteId;
        RETURN @@ROWCOUNT;
    END
    ');
    PRINT 'Stored procedure 100_DeleteSiteOperatingDaysBySiteId creado';
END

PRINT '=============================================';
PRINT 'STORED PROCEDURES ADICIONALES CREADOS EXITOSAMENTE';
PRINT '=============================================';

COMMIT TRANSACTION;
