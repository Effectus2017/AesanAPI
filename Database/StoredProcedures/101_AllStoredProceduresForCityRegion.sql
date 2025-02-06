-- Procedimientos almacenados para Region
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

CREATE OR ALTER PROCEDURE [101_GetRegionById]
    @regionId INT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE Id = @regionId;
END;
GO

CREATE OR ALTER PROCEDURE [101_InsertRegion]
    @name VARCHAR(50),
    @id INT OUTPUT
AS
BEGIN
    INSERT INTO Region (Name)
    VALUES (@name);
    
    SET @id = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE [101_UpdateRegion]
    @id INT,
    @name VARCHAR(50)
AS
BEGIN
    UPDATE Region
    SET Name = @name
    WHERE Id = @id;
    
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE [101_DeleteRegion]
    @id INT
AS
BEGIN
    DELETE FROM Region
    WHERE Id = @id;
    
    SELECT @@ROWCOUNT;
END;
GO

-- Procedimientos almacenados para City
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

CREATE OR ALTER PROCEDURE [101_GetCityById]
    @cityId INT
AS
BEGIN
    SELECT c.*, r.Name as RegionName
    FROM City c
    INNER JOIN Region r ON c.RegionId = r.Id
    WHERE c.Id = @cityId;
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

CREATE OR ALTER PROCEDURE [101_InsertCity]
    @name VARCHAR(50),
    @regionId INT,
    @id INT OUTPUT
AS
BEGIN
    INSERT INTO City (Name, RegionId)
    VALUES (@name, @regionId);
    
    SET @id = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE [101_UpdateCity]
    @id INT,
    @name VARCHAR(50),
    @regionId INT
AS
BEGIN
    UPDATE City
    SET Name = @name,
        RegionId = @regionId
    WHERE Id = @id;
    
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE [101_DeleteCity]
    @id INT
AS
BEGIN
    DELETE FROM City
    WHERE Id = @id;
    
    SELECT @@ROWCOUNT;
END;
GO 