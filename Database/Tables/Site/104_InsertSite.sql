-- =============================================
-- Stored Procedure: 104_InsertSite
-- Descripción: Inserta un nuevo sitio en la base de datos
-- Reemplaza: 104_InsertSchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[104_InsertSite]
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
    @isActive BIT = NULL,
    @isMainSite BIT = NULL,
    @inactiveJustification NVARCHAR(500) = NULL,
    @inactiveDate DATETIME = NULL,
    @organizedAthleticPrograms BIT = NULL,
    @atRiskService BIT = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Site
        (
        AgencyId, Name, StartDate, Address, CityId, RegionId, ZipCode, Latitude, Longitude,
        PostalAddress, PostalCityId, PostalRegionId, PostalZipCode, SameAsPhysicalAddress,
        OrganizationTypeId, CenterTypeId, NonProfit, BaseYear, RenewalYear, OperatingFromDate, OperatingToDate, OperatingDaysCalculated,
        KitchenTypeId, GroupTypeId, DeliveryTypeId, SponsorTypeId, ApplicantTypeId, ResidentialTypeId, OperatingPolicyId, AreaTypeId, LocationTypeId,
        HasWarehouse, HasDiningRoom, AdministratorAuthorizedName, SitePhone, Extension, MobilePhone,
        CommunityId, WalkersId, SiteTypeId, ExperienceId, ReviewResultId, ReviewDate, ReviewJustification,
        SiteCode, GeneralEnrollment, SiteNumber, ServiceTime, IsActive, IsMainSite, InactiveJustification, InactiveDate,
        OrganizedAthleticPrograms, AtRiskService, CreatedAt
        )
    VALUES
        (
            @agencyId, @name, @startDate, @address, @cityId, @regionId, @zipCode, @latitude, @longitude,
            @postalAddress, @postalCityId, @postalRegionId, @postalZipCode, @sameAsPhysicalAddress,
            @organizationTypeId, @centerTypeId, @nonProfit, @baseYear, @renewalYear, @operatingFromDate, @operatingToDate, @operatingDaysCalculated,
            @kitchenTypeId, @groupTypeId, @deliveryTypeId, @sponsorTypeId, @applicantTypeId, @residentialTypeId, @operatingPolicyId, @areaTypeId, @locationTypeId,
            @hasWarehouse, @hasDiningRoom, @administratorAuthorizedName, @sitePhone, @extension, @mobilePhone,
            @communityId, @walkersId, @siteTypeId, @experienceId, @reviewResultId, @reviewDate, @reviewJustification,
            @siteCode, @generalEnrollment, @siteNumber, @serviceTime, @isActive, @isMainSite, @inactiveJustification, @inactiveDate,
            @organizedAthleticPrograms, @atRiskService, GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
END;
