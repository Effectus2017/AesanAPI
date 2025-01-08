CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id,
        s.Name,
        s.EducationLevelId,
        el.Name AS EducationLevelName,
        s.OperatingPeriodId,
        op.Name AS OperatingPeriodName,
        s.Address,
        s.CityId,
        c.Name AS CityName,
        s.RegionId,
        r.Name AS RegionName,
        s.ZipCode,
        s.OrganizationTypeId,
        ot.Name AS OrganizationTypeName
    FROM School s
    LEFT JOIN EducationLevel el ON s.EducationLevelId = el.Id
    LEFT JOIN OperatingPeriod op ON s.OperatingPeriodId = op.Id
    LEFT JOIN City c ON s.CityId = c.Id
    LEFT JOIN Region r ON s.RegionId = r.Id
    LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
    WHERE s.Id = @id;
END;
GO

-- =============================================
-- Get All Schools
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_GetSchools]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id,
        s.Name,
        s.EducationLevelId,
        el.Name AS EducationLevelName,
        s.OperatingPeriodId,
        op.Name AS OperatingPeriodName,
        s.Address,
        s.CityId,
        c.Name AS CityName,
        s.RegionId,
        r.Name AS RegionName,
        s.ZipCode,
        s.OrganizationTypeId,
        ot.Name AS OrganizationTypeName
    FROM School s
    LEFT JOIN EducationLevel el ON s.EducationLevelId = el.Id
    LEFT JOIN OperatingPeriod op ON s.OperatingPeriodId = op.Id
    LEFT JOIN City c ON s.CityId = c.Id
    LEFT JOIN Region r ON s.RegionId = r.Id
    LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
    WHERE (@alls = 1) OR (@name IS NULL OR s.Name LIKE '%' + @name + '%')
    ORDER BY s.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM School s
    WHERE (@alls = 1) OR (@name IS NULL OR s.Name LIKE '%' + @name + '%');
END;
GO

-- =============================================
-- Insert School
-- =============================================
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
GO

-- =============================================
-- Update School
-- =============================================
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
GO

-- =============================================
-- Delete School
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSchool]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Primero eliminamos las relaciones en las tablas dependientes
    DELETE FROM SchoolFacility WHERE SchoolId = @id;
    DELETE FROM SchoolMeal WHERE SchoolId = @id;

    -- Luego eliminamos la escuela
    DELETE FROM School WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 