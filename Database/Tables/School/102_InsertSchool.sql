-- DEPRECATED: Esta versión del SP InsertSchool ha sido reemplazada por una nueva versión. No modificar ni usar para nuevas migraciones.
CREATE OR ALTER PROCEDURE [dbo].[102_InsertSchool]
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
    @educationLevelId INT,
    @operatingDays INT = NULL,
    @kitchenTypeId INT = NULL,
    @groupTypeId INT = NULL,
    @deliveryTypeId INT = NULL,
    @sponsorTypeId INT = NULL,
    @applicantTypeId INT = NULL,
    @residentialTypeId INT = NULL,
    @operatingPolicyId INT = NULL,
    @hasWarehouse BIT = NULL,
    @hasDiningRoom BIT = NULL,
    @administratorAuthorizedName NVARCHAR(255) = NULL,
    @sitePhone NVARCHAR(20) = NULL,
    @extension NVARCHAR(10) = NULL,
    @mobilePhone NVARCHAR(20) = NULL,
    @breakfast BIT = NULL,
    @breakfastFrom TIME = NULL,
    @breakfastTo TIME = NULL,
    @lunch BIT = NULL,
    @lunchFrom TIME = NULL,
    @lunchTo TIME = NULL,
    @snack BIT = NULL,
    @snackFrom TIME = NULL,
    @snackTo TIME = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @isMainSchool BIT = 1;

    -- Lógica para asegurar que solo una escuela principal por agencia
    IF EXISTS (SELECT 1
    FROM School
    WHERE AgencyId = @agencyId AND IsMainSchool = 1 AND IsActive = 1)
    BEGIN
        SET @isMainSchool = 0;
    END
    ELSE
    BEGIN
        SET @isMainSchool = 1;
    END

    INSERT INTO School
        (
        AgencyId, Name, StartDate, Address, CityId, RegionId, ZipCode, Latitude, Longitude,
        PostalAddress, PostalCityId, PostalRegionId, PostalZipCode, SameAsPhysicalAddress,
        OrganizationTypeId, CenterTypeId, NonProfit, BaseYear, RenewalYear, EducationLevelId, OperatingDays,
        KitchenTypeId, GroupTypeId, DeliveryTypeId, SponsorTypeId, ApplicantTypeId, ResidentialTypeId, OperatingPolicyId,
        HasWarehouse, HasDiningRoom, AdministratorAuthorizedName, SitePhone, Extension, MobilePhone,
        Breakfast, BreakfastFrom, BreakfastTo, Lunch, LunchFrom, LunchTo, Snack, SnackFrom, SnackTo, IsMainSchool,
        IsActive, CreatedAt
        )
    VALUES
        (
            @agencyId, @name, @startDate, @address, @cityId, @regionId, @zipCode, @latitude, @longitude,
            @postalAddress, @postalCityId, @postalRegionId, @postalZipCode, @sameAsPhysicalAddress,
            @organizationTypeId, @centerTypeId, @nonProfit, @baseYear, @renewalYear, @educationLevelId, @operatingDays,
            @kitchenTypeId, @groupTypeId, @deliveryTypeId, @sponsorTypeId, @applicantTypeId, @residentialTypeId, @operatingPolicyId,
            @hasWarehouse, @hasDiningRoom, @administratorAuthorizedName, @sitePhone, @extension, @mobilePhone,
            @breakfast, @breakfastFrom, @breakfastTo, @lunch, @lunchFrom, @lunchTo, @snack, @snackFrom, @snackTo, @isMainSchool,
            1, GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
END; 