-- Paso 1: Crear tablas temporales para mantener los datos existentes
BEGIN TRANSACTION;

BEGIN TRY
    -- Obtener los nombres de todas las foreign keys que referencian a City y Region
    CREATE TABLE #ForeignKeys
    (
        TableName NVARCHAR(128),
        ConstraintName NVARCHAR(128)
    );

    INSERT INTO #ForeignKeys
    SELECT 
        OBJECT_NAME(parent_object_id) AS TableName,
        name AS ConstraintName
    FROM sys.foreign_keys
    WHERE referenced_object_id IN (OBJECT_ID('City'), OBJECT_ID('Region'));

    -- Eliminar todas las foreign keys encontradas
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @TableName NVARCHAR(128);
    DECLARE @ConstraintName NVARCHAR(128);

    DECLARE fk_cursor CURSOR FOR
    SELECT TableName, ConstraintName
    FROM #ForeignKeys;

    OPEN fk_cursor;
    FETCH NEXT FROM fk_cursor INTO @TableName, @ConstraintName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'ALTER TABLE ' + QUOTENAME(@TableName) + 
                   ' DROP CONSTRAINT ' + QUOTENAME(@ConstraintName);
        EXEC sp_executesql @sql;
        FETCH NEXT FROM fk_cursor INTO @TableName, @ConstraintName;
    END

    CLOSE fk_cursor;
    DEALLOCATE fk_cursor;

    -- Crear tablas temporales para los datos
    CREATE TABLE #TempCity
    (
        Id INT,
        Name VARCHAR(50),
        RegionId INT
    );

    CREATE TABLE #TempRegion
    (
        Id INT,
        Name VARCHAR(50)
    );

    -- Paso 2: Copiar datos existentes a las tablas temporales
    INSERT INTO #TempRegion (Id, Name)
    SELECT Id, Name FROM Region;

    -- Modificar la consulta para obtener la relación correcta
    WITH CityRegionMapping AS (
        SELECT 
            c.Id AS CityId,
            c.Name AS CityName,
            r.Id AS RegionId
        FROM City c
        LEFT JOIN Region r ON r.CityId = c.Id
    )
    INSERT INTO #TempCity (Id, Name, RegionId)
    SELECT CityId, CityName, RegionId
    FROM CityRegionMapping;

    -- Obtener el máximo ID de cada tabla para el IDENTITY
    DECLARE @MaxRegionId INT = (SELECT ISNULL(MAX(Id), 0) FROM #TempRegion);
    DECLARE @MaxCityId INT = (SELECT ISNULL(MAX(Id), 0) FROM #TempCity);

    -- Ahora podemos eliminar las tablas con seguridad
    DROP TABLE Region;
    DROP TABLE City;

    -- Paso 4: Crear las nuevas tablas con la relación correcta
    CREATE TABLE Region
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name VARCHAR(50) NOT NULL
    );

    CREATE TABLE City
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        RegionId INT,
        Name VARCHAR(50) NOT NULL,
        FOREIGN KEY (RegionId) REFERENCES Region(Id)
    );

    -- Paso 5: Restaurar los datos
    -- Primero insertamos las regiones
    SET IDENTITY_INSERT Region ON;
    INSERT INTO Region (Id, Name)
    SELECT DISTINCT Id, Name 
    FROM #TempRegion 
    ORDER BY Id;
    SET IDENTITY_INSERT Region OFF;

    -- Reseed IDENTITY para Region
    DBCC CHECKIDENT ('Region', RESEED, @MaxRegionId);

    -- Luego insertamos las ciudades
    SET IDENTITY_INSERT City ON;
    WITH UniqueCities AS (
        SELECT DISTINCT Id, Name, RegionId,
               ROW_NUMBER() OVER (PARTITION BY Id ORDER BY Id) as rn
        FROM #TempCity
    )
    INSERT INTO City (Id, Name, RegionId)
    SELECT Id, Name, RegionId
    FROM UniqueCities
    WHERE rn = 1
    ORDER BY Id;
    SET IDENTITY_INSERT City OFF;

    -- Reseed IDENTITY para City
    DBCC CHECKIDENT ('City', RESEED, @MaxCityId);

    -- Paso 6: Limpiar las tablas temporales
    DROP TABLE #TempCity;
    DROP TABLE #TempRegion;
    DROP TABLE #ForeignKeys;

    -- Paso 7: Recrear las foreign keys en Agency con nombres específicos
    ALTER TABLE Agency ADD CONSTRAINT FK_Agency_City FOREIGN KEY (CityId) REFERENCES City(Id);
    ALTER TABLE Agency ADD CONSTRAINT FK_Agency_Region FOREIGN KEY (RegionId) REFERENCES Region(Id);

    -- Paso 8: Verificar la integridad de los datos
    DECLARE @OrphanedRecords INT;
    
    SELECT @OrphanedRecords = COUNT(1)
    FROM Agency a
    LEFT JOIN City c ON a.CityId = c.Id
    LEFT JOIN Region r ON a.RegionId = r.Id
    WHERE c.Id IS NULL OR r.Id IS NULL;

    IF @OrphanedRecords > 0
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(200) = 'Se encontraron ' + CAST(@OrphanedRecords AS NVARCHAR(10)) + ' referencias huérfanas en la tabla Agency';
        RAISERROR(@ErrorMsg, 16, 1);
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF (SELECT CURSOR_STATUS('global','fk_cursor')) >= -1
    BEGIN
        CLOSE fk_cursor;
        DEALLOCATE fk_cursor;
    END
    
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();

    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO

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