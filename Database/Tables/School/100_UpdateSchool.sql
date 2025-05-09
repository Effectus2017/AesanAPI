CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchool]
    @id INT,
    @name NVARCHAR(255),
    @educationLevelId INT,
    @operatingPeriodId INT,
    @address NVARCHAR(255),
    @cityId INT,
    @regionId INT,
    @zipCode NVARCHAR(10),
    @organizationTypeId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE School
    SET Name = @name,
        EducationLevelId = @educationLevelId,
        OperatingPeriodId = @operatingPeriodId,
        Address = @address,
        CityId = @cityId,
        RegionId = @regionId,
        ZipCode = @zipCode,
        OrganizationTypeId = @organizationTypeId
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END; 