CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchool]
    @name NVARCHAR(255),
    @educationLevelId INT,
    @operatingPeriodId INT,
    @address NVARCHAR(255),
    @cityId INT,
    @regionId INT,
    @zipCode NVARCHAR(10),
    @organizationTypeId INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO School (
        Name,
        EducationLevelId,
        OperatingPeriodId,
        Address,
        CityId,
        RegionId,
        ZipCode,
        OrganizationTypeId
    )
    VALUES (
        @name,
        @educationLevelId,
        @operatingPeriodId,
        @address,
        @cityId,
        @regionId,
        @zipCode,
        @organizationTypeId
    );

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 