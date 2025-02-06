-- Procedimientos para Regiones
CREATE OR ALTER PROCEDURE [100_GetRegions]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id,
           r.Name,
           r.IsActive,
           r.CreatedAt,
           r.UpdatedAt
    FROM Region r
    WHERE (@alls = 1 OR r.Name LIKE '%' + @name + '%')
        AND r.IsActive = 1
    ORDER BY r.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Region r
    WHERE (@alls = 1 OR r.Name LIKE '%' + @name + '%')
        AND r.IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE [100_GetRegionById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id,
           r.Name,
           r.IsActive,
           r.CreatedAt,
           r.UpdatedAt
    FROM Region r
    WHERE r.Id = @id AND r.IsActive = 1;
END;
GO

-- Procedimientos para Ciudades
CREATE OR ALTER PROCEDURE [100_GetCities]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.Id,
           c.Name,
           c.IsActive,
           c.CreatedAt,
           c.UpdatedAt
    FROM City c
    WHERE (@alls = 1 OR c.Name LIKE '%' + @name + '%')
        AND c.IsActive = 1
    ORDER BY c.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM City c
    WHERE (@alls = 1 OR c.Name LIKE '%' + @name + '%')
        AND c.IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE [100_GetCityById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.Id,
           c.Name,
           c.IsActive,
           c.CreatedAt,
           c.UpdatedAt
    FROM City c
    WHERE c.Id = @id AND c.IsActive = 1;
END;
GO

-- Procedimientos para CityRegion
CREATE OR ALTER PROCEDURE [100_GetCitiesByRegionId]
    @regionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.Id,
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

    SELECT COUNT(*) AS TotalCount
    FROM City c
    INNER JOIN CityRegion cr ON c.Id = cr.CityId
    WHERE cr.RegionId = @regionId 
        AND c.IsActive = 1 
        AND cr.IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE [100_GetRegionsByCityId]
    @cityId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id,
           r.Name,
           r.IsActive,
           r.CreatedAt,
           r.UpdatedAt
    FROM Region r
    INNER JOIN CityRegion cr ON r.Id = cr.RegionId
    WHERE cr.CityId = @cityId 
        AND r.IsActive = 1 
        AND cr.IsActive = 1
    ORDER BY r.Name;

    SELECT COUNT(*) AS TotalCount
    FROM Region r
    INNER JOIN CityRegion cr ON r.Id = cr.RegionId
    WHERE cr.CityId = @cityId 
        AND r.IsActive = 1 
        AND cr.IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE [100_GetCityRegionById]
    @cityId INT,
    @regionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT cr.Id,
           cr.CityId,
           cr.RegionId,
           cr.IsActive,
           cr.CreatedAt,
           cr.UpdatedAt,
           c.Name AS CityName,
           r.Name AS RegionName
    FROM CityRegion cr
    INNER JOIN City c ON cr.CityId = c.Id
    INNER JOIN Region r ON cr.RegionId = r.Id
    WHERE cr.CityId = @cityId 
        AND cr.RegionId = @regionId 
        AND cr.IsActive = 1;
END;
GO

-- Procedimiento para obtener todas las relaciones CityRegion
CREATE OR ALTER PROCEDURE [100_GetAllCityRegions]
    @take INT,
    @skip INT,
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT cr.Id,
           cr.CityId,
           cr.RegionId,
           cr.IsActive,
           cr.CreatedAt,
           cr.UpdatedAt,
           c.Name AS CityName,
           r.Name AS RegionName
    FROM CityRegion cr
    INNER JOIN City c ON cr.CityId = c.Id
    INNER JOIN Region r ON cr.RegionId = r.Id
    WHERE (@alls = 1 OR cr.IsActive = 1)
    ORDER BY c.Name, r.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM CityRegion cr
    WHERE (@alls = 1 OR cr.IsActive = 1);
END;
GO 