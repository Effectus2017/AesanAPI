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
    WHERE (@alls = 1 OR r.IsActive = 1)
        AND (@name IS NULL OR r.Name LIKE '%' + @name + '%')
    ORDER BY r.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Region r
    WHERE (@alls = 1 OR r.IsActive = 1)
        AND (@name IS NULL OR r.Name LIKE '%' + @name + '%');
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

CREATE OR ALTER PROCEDURE [100_InsertRegion]
    @name NVARCHAR(50),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Region (Name, IsActive, CreatedAt)
    VALUES (@name, 1, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

CREATE OR ALTER PROCEDURE [100_UpdateRegion]
    @id INT,
    @name NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Region
    SET Name = @name,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE [100_DeleteRegion]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Region
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
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
    WHERE (@alls = 1 OR c.IsActive = 1)
        AND (@name IS NULL OR c.Name LIKE '%' + @name + '%')
    ORDER BY c.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM City c
    WHERE (@alls = 1 OR c.IsActive = 1)
        AND (@name IS NULL OR c.Name LIKE '%' + @name + '%');
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

CREATE OR ALTER PROCEDURE [100_InsertCity]
    @name NVARCHAR(50),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO City (Name, IsActive, CreatedAt)
    VALUES (@name, 1, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

CREATE OR ALTER PROCEDURE [100_UpdateCity]
    @id INT,
    @name NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE City
    SET Name = @name,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE [100_DeleteCity]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE City
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
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

CREATE OR ALTER PROCEDURE [100_InsertCityRegion]
    @cityId INT,
    @regionId INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO CityRegion (CityId, RegionId, IsActive, CreatedAt)
    VALUES (@cityId, @regionId, 1, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

CREATE OR ALTER PROCEDURE [100_DeleteCityRegion]
    @cityId INT,
    @regionId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE CityRegion
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE CityId = @cityId 
        AND RegionId = @regionId 
        AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO