-- DEPRECATED: Esta versión del SP InsertSchool ha sido reemplazada por una nueva versión. No modificar ni usar para nuevas migraciones.
CREATE OR ALTER PROCEDURE [dbo].[101_InsertSchool]
    @Name NVARCHAR(255),
    @StartDate DATE = NULL,
    @Address NVARCHAR(255),
    @PostalAddress NVARCHAR(255) = NULL,
    @ZipCode NVARCHAR(20),
    @CityId INT,
    @RegionId INT,
    @AreaCode NVARCHAR(10) = NULL,
    @AdminFullName NVARCHAR(255) = NULL,
    @Phone NVARCHAR(20) = NULL,
    @PhoneExtension NVARCHAR(10) = NULL,
    @Mobile NVARCHAR(20) = NULL,
    @BaseYear INT = NULL,
    @NextRenewalYear INT = NULL,
    @OrganizationTypeId INT,
    @EducationLevelId INT,
    @OperatingPeriodId INT,
    @KitchenTypeId INT = NULL,
    @GroupTypeId INT = NULL,
    @DeliveryTypeId INT = NULL,
    @SponsorTypeId INT = NULL,
    @ApplicantTypeId INT = NULL,
    @OperatingPolicyId INT = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO School
        (
        Name, StartDate, Address, PostalAddress, ZipCode, CityId, RegionId, AreaCode, AdminFullName, Phone, PhoneExtension, Mobile, BaseYear, NextRenewalYear, OrganizationTypeId, EducationLevelId, OperatingPeriodId, KitchenTypeId, GroupTypeId, DeliveryTypeId, SponsorTypeId, ApplicantTypeId, OperatingPolicyId, IsActive, CreatedAt
        )
    VALUES
        (
            @Name, @StartDate, @Address, @PostalAddress, @ZipCode, @CityId, @RegionId, @AreaCode, @AdminFullName, @Phone, @PhoneExtension, @Mobile, @BaseYear, @NextRenewalYear, @OrganizationTypeId, @EducationLevelId, @OperatingPeriodId, @KitchenTypeId, @GroupTypeId, @DeliveryTypeId, @SponsorTypeId, @ApplicantTypeId, @OperatingPolicyId, 1, GETDATE()
    );

    SET @Id = SCOPE_IDENTITY();
END; 