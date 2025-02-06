-- Actualizar los stored procedures afectados
CREATE OR ALTER PROCEDURE [101_GetRegions]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Region
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%');
END;
GO

CREATE OR ALTER PROCEDURE [101_GetCities]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT,
    @regionId INT = NULL
AS
BEGIN
    SELECT c.*, r.Name as RegionName
    FROM City c
    INNER JOIN Region r ON c.RegionId = r.Id
    WHERE (@alls = 1 OR c.Name LIKE '%' + @name + '%')
    AND (@regionId IS NULL OR c.RegionId = @regionId)
    ORDER BY c.Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM City c
    WHERE (@alls = 1 OR c.Name LIKE '%' + @name + '%')
    AND (@regionId IS NULL OR c.RegionId = @regionId);
END;
GO

CREATE OR ALTER PROCEDURE [101_GetCitiesByRegionId]
    @regionId INT
AS
BEGIN
    SELECT c.*, r.Name as RegionName
    FROM City c
    INNER JOIN Region r ON c.RegionId = r.Id
    WHERE c.RegionId = @regionId
    ORDER BY c.Name;

    SELECT COUNT(*) AS TotalCount
    FROM City c
    WHERE c.RegionId = @regionId;
END;
GO 