-- DEPRECATED: Esta versión del SP UpdateSchool ha sido reemplazada por una nueva versión. No modificar ni usar para nuevas migraciones.
CREATE OR ALTER PROCEDURE [dbo].[101_UpdateSchool]
    @Id INT,
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
    @OperatingPolicyId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE School
    SET Name = @Name,
        StartDate = @StartDate,
        Address = @Address,
        PostalAddress = @PostalAddress,
        ZipCode = @ZipCode,
        CityId = @CityId,
        RegionId = @RegionId,
        AreaCode = @AreaCode,
        AdminFullName = @AdminFullName,
        Phone = @Phone,
        PhoneExtension = @PhoneExtension,
        Mobile = @Mobile,
        BaseYear = @BaseYear,
        NextRenewalYear = @NextRenewalYear,
        OrganizationTypeId = @OrganizationTypeId,
        EducationLevelId = @EducationLevelId,
        OperatingPeriodId = @OperatingPeriodId,
        KitchenTypeId = @KitchenTypeId,
        GroupTypeId = @GroupTypeId,
        DeliveryTypeId = @DeliveryTypeId,
        SponsorTypeId = @SponsorTypeId,
        ApplicantTypeId = @ApplicantTypeId,
        OperatingPolicyId = @OperatingPolicyId,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END; 