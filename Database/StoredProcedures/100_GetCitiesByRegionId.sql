CREATE OR ALTER PROCEDURE [dbo].[100_GetCitiesByRegionId]
    @regionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        c.Id,
        c.Name,
        c.IsActive,
        c.CreatedAt,
        c.UpdatedAt
    FROM City c
    INNER JOIN CityRegion cr ON c.Id = cr.CityId
    WHERE cr.RegionId = @regionId
        AND c.IsActive = 1
        AND cr.IsActive = 1
    ORDER BY c.Name;

    SELECT COUNT(DISTINCT c.Id)
    FROM City c
    INNER JOIN CityRegion cr ON c.Id = cr.CityId
    WHERE cr.RegionId = @regionId
        AND c.IsActive = 1
        AND cr.IsActive = 1;
END;
GO 