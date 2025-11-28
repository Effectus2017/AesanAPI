-- =============================================
-- Stored Procedure: 104_UpdateSite
-- Descripción: Actualiza un sitio existente en la base de datos
-- Reemplaza: 104_UpdateSchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[104_UpdateSite]
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
    @communityId INT = NULL,
    @walkersId INT = NULL,
    @siteTypeId INT = NULL,
    @siteLocationId INT = NULL,
    @experienceId INT = NULL,
    @reviewResultId INT = NULL,
    @reviewDate DATETIME = NULL,
    @reviewJustification NVARCHAR(500) = NULL,
    @siteCode NVARCHAR(20) = NULL,
    @siteNumber INT = NULL,
    @isActive BIT = NULL,
    @inactiveJustification NVARCHAR(500) = NULL,
    @inactiveDate DATETIME = NULL,
    @generalEnrollment INT = NULL,
    @serviceTime DATETIME = NULL,
    @organizedAthleticPrograms BIT = NULL,
    @atRiskService BIT = NULL,
    @publicAllianceContractId INT = NULL,
    @isDayCareHomeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE Site 
        SET 
            AgencyId = ISNULL(@agencyId, AgencyId),
            Name = ISNULL(@name, Name),
            StartDate = ISNULL(@startDate, StartDate),
            Address = ISNULL(@address, Address),
            CityId = ISNULL(@cityId, CityId),
            RegionId = ISNULL(@regionId, RegionId),
            ZipCode = ISNULL(@zipCode, ZipCode),
            Latitude = ISNULL(@latitude, Latitude),
            Longitude = ISNULL(@longitude, Longitude),
            PostalAddress = ISNULL(@postalAddress, PostalAddress),
            PostalCityId = ISNULL(@postalCityId, PostalCityId),
            PostalRegionId = ISNULL(@postalRegionId, PostalRegionId),
            PostalZipCode = ISNULL(@postalZipCode, PostalZipCode),
            SameAsPhysicalAddress = ISNULL(@sameAsPhysicalAddress, SameAsPhysicalAddress),
            OrganizationTypeId = ISNULL(@organizationTypeId, OrganizationTypeId),
            CenterTypeId = ISNULL(@centerTypeId, CenterTypeId),
            AreaTypeId = ISNULL(@areaTypeId, AreaTypeId),
            LocationTypeId = ISNULL(@locationTypeId, LocationTypeId),
            NonProfit = ISNULL(@nonProfit, NonProfit),
            BaseYear = ISNULL(@baseYear, BaseYear),
            RenewalYear = ISNULL(@renewalYear, RenewalYear),
            OperatingFromDate = ISNULL(@operatingFromDate, OperatingFromDate),
            OperatingToDate = ISNULL(@operatingToDate, OperatingToDate),
            OperatingDaysCalculated = ISNULL(@operatingDaysCalculated, OperatingDaysCalculated),
            KitchenTypeId = ISNULL(@kitchenTypeId, KitchenTypeId),
            GroupTypeId = ISNULL(@groupTypeId, GroupTypeId),
            DeliveryTypeId = ISNULL(@deliveryTypeId, DeliveryTypeId),
            SponsorTypeId = ISNULL(@sponsorTypeId, SponsorTypeId),
            ApplicantTypeId = ISNULL(@applicantTypeId, ApplicantTypeId),
            ResidentialTypeId = ISNULL(@residentialTypeId, ResidentialTypeId),
            OperatingPolicyId = ISNULL(@operatingPolicyId, OperatingPolicyId),
            HasWarehouse = ISNULL(@hasWarehouse, HasWarehouse),
            HasDiningRoom = ISNULL(@hasDiningRoom, HasDiningRoom),
            CommunityId = ISNULL(@communityId, CommunityId),
            WalkersId = ISNULL(@walkersId, WalkersId),
            SiteTypeId = ISNULL(@siteTypeId, SiteTypeId),
            SiteLocationId = ISNULL(@siteLocationId, SiteLocationId),
            ExperienceId = ISNULL(@experienceId, ExperienceId),
            ReviewResultId = ISNULL(@reviewResultId, ReviewResultId),
            ReviewDate = ISNULL(@reviewDate, ReviewDate),
            ReviewJustification = ISNULL(@reviewJustification, ReviewJustification),
            SiteCode = ISNULL(@siteCode, SiteCode),
            SiteNumber = ISNULL(@siteNumber, SiteNumber),
            IsActive = ISNULL(@isActive, IsActive),
            InactiveJustification = ISNULL(@inactiveJustification, InactiveJustification),
            InactiveDate = ISNULL(@inactiveDate, InactiveDate),
            GeneralEnrollment = ISNULL(@generalEnrollment, GeneralEnrollment),
            ServiceTime = ISNULL(@serviceTime, ServiceTime),
            OrganizedAthleticPrograms = ISNULL(@organizedAthleticPrograms, OrganizedAthleticPrograms),
            AtRiskService = ISNULL(@atRiskService, AtRiskService),
            PublicAllianceContractId = ISNULL(@publicAllianceContractId, PublicAllianceContractId),
            IsDayCareHomeId = ISNULL(@isDayCareHomeId, IsDayCareHomeId),
            UpdatedAt = GETDATE()
        WHERE Id = @id;

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;