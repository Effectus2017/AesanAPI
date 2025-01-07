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