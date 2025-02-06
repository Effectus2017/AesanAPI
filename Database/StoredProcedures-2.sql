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

-- Procedimiento para obtener todas las agencias con información de ciudad y región
CREATE OR ALTER PROCEDURE [100_GetAgencies]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.Id,
        a.Name,
        a.StatusId,
        -- Datos de la Agencia
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        -- Dirección Física
        a.Address,
        a.ZipCode,
        -- Ciudad y Región
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        -- Dirección Postal
        a.PostalAddress,
        a.PostalZipCode,
        pc.Id AS PostalCityId,
        pc.Name AS PostalCityName,
        pr.Id AS PostalRegionId,
        pr.Name AS PostalRegionName,
        -- Teléfono y Email
        a.Phone,
        a.Email,
        -- Otros datos
        a.NonProfit,
        a.FederalFundsDenied,
        a.StateFundsDenied,
        a.OrganizedAthleticPrograms,
        -- Estado
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Auditoría
        a.IsActive,
        a.CreatedAt,
        a.UpdatedAt,
        -- Imagen y Justificación
        a.ImageURL,
        a.RejectionJustification
    FROM Agency a
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
    WHERE (@alls = 1 OR a.IsActive = 1)
        AND (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId OR a.PostalRegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId OR a.PostalCityId = @cityId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
    ORDER BY a.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Obtener programas asociados si se especifica un ID de programa
    IF @programId IS NOT NULL
    BEGIN
        SELECT 
            p.Id,
            p.Name,
            p.Description,
            ap.AgencyId
        FROM Program p
            INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
        WHERE ap.AgencyId IN (
            SELECT a.Id
            FROM Agency a
            WHERE (@alls = 1 OR a.IsActive = 1)
                AND (@name IS NULL OR a.Name LIKE '%' + @name + '%')
                AND (@regionId IS NULL OR a.RegionId = @regionId OR a.PostalRegionId = @regionId)
                AND (@cityId IS NULL OR a.CityId = @cityId OR a.PostalCityId = @cityId)
                AND (@statusId IS NULL OR a.StatusId = @statusId)
        ) AND p.Id = @programId;
    END

    -- Obtener el conteo total
    SELECT COUNT(*) AS TotalCount
    FROM Agency a
    WHERE (@alls = 1 OR a.IsActive = 1)
        AND (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId OR a.PostalRegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId OR a.PostalCityId = @cityId)
        AND (@statusId IS NULL OR a.StatusId = @statusId);
END;
GO

-- Procedimiento para insertar una agencia
CREATE OR ALTER PROCEDURE [100_InsertAgency]
    @Name NVARCHAR(MAX),
    @StatusId INT,
    -- Datos de la Agencia
    @SdrNumber NVARCHAR(255),
    @UieNumber NVARCHAR(255),
    @EinNumber NVARCHAR(255),
    -- Dirección Física
    @Address NVARCHAR(255),
    @ZipCode INT,
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    -- Dirección Postal
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT,
    @PostalCityId INT,
    @PostalRegionId INT,
    -- Teléfono y Email
    @Phone NVARCHAR(50),
    @Email NVARCHAR(255),
    -- Otros datos
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    -- Justificación e Imagen
    @RejectionJustification NVARCHAR(MAX) = NULL,
    @ImageURL NVARCHAR(MAX) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que las ciudades y regiones existan y estén activas
    IF NOT EXISTS (SELECT 1 FROM City WHERE Id = @CityId AND IsActive = 1)
        OR NOT EXISTS (SELECT 1 FROM Region WHERE Id = @RegionId AND IsActive = 1)
        OR NOT EXISTS (SELECT 1 FROM City WHERE Id = @PostalCityId AND IsActive = 1)
        OR NOT EXISTS (SELECT 1 FROM Region WHERE Id = @PostalRegionId AND IsActive = 1)
    BEGIN
        RAISERROR ('Una o más ciudades o regiones especificadas no existen o no están activas', 16, 1);
        RETURN -1;
    END

    INSERT INTO Agency (
        Name, StatusId,
        SdrNumber, UieNumber, EinNumber,
        Address, ZipCode, CityId, RegionId, Latitude, Longitude,
        PostalAddress, PostalZipCode, PostalCityId, PostalRegionId,
        Phone, Email,
        NonProfit, FederalFundsDenied, StateFundsDenied, OrganizedAthleticPrograms,
        RejectionJustification, ImageURL,
        IsActive, CreatedAt
    )
    VALUES (
        @Name, @StatusId,
        @SdrNumber, @UieNumber, @EinNumber,
        @Address, @ZipCode, @CityId, @RegionId, @Latitude, @Longitude,
        @PostalAddress, @PostalZipCode, @PostalCityId, @PostalRegionId,
        @Phone, @Email,
        @NonProfit, @FederalFundsDenied, @StateFundsDenied, @OrganizedAthleticPrograms,
        @RejectionJustification, @ImageURL,
        1, GETDATE()
    );

    SET @Id = SCOPE_IDENTITY();
    RETURN @Id;
END;
GO

-- Procedimiento para actualizar una agencia
CREATE OR ALTER PROCEDURE [100_UpdateAgency]
    @Id INT,
    @Name NVARCHAR(MAX),
    @StatusId INT,
    -- Datos de la Agencia
    @SdrNumber NVARCHAR(255),
    @UieNumber NVARCHAR(255),
    @EinNumber NVARCHAR(255),
    -- Dirección Física
    @Address NVARCHAR(255),
    @ZipCode INT,
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    -- Dirección Postal
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT,
    @PostalCityId INT,
    @PostalRegionId INT,
    -- Teléfono y Email
    @Phone NVARCHAR(50),
    @Email NVARCHAR(255),
    -- Otros datos
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    -- Justificación e Imagen
    @RejectionJustification NVARCHAR(MAX) = NULL,
    @ImageURL NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Validar que las ciudades y regiones existan y estén activas
    IF NOT EXISTS (SELECT 1 FROM City WHERE Id = @CityId AND IsActive = 1)
        OR NOT EXISTS (SELECT 1 FROM Region WHERE Id = @RegionId AND IsActive = 1)
        OR NOT EXISTS (SELECT 1 FROM City WHERE Id = @PostalCityId AND IsActive = 1)
        OR NOT EXISTS (SELECT 1 FROM Region WHERE Id = @PostalRegionId AND IsActive = 1)
    BEGIN
        RAISERROR ('Una o más ciudades o regiones especificadas no existen o no están activas', 16, 1);
        RETURN -1;
    END

    UPDATE Agency
    SET Name = @Name,
        StatusId = @StatusId,
        SdrNumber = @SdrNumber,
        UieNumber = @UieNumber,
        EinNumber = @EinNumber,
        Address = @Address,
        ZipCode = @ZipCode,
        CityId = @CityId,
        RegionId = @RegionId,
        Latitude = @Latitude,
        Longitude = @Longitude,
        PostalAddress = @PostalAddress,
        PostalZipCode = @PostalZipCode,
        PostalCityId = @PostalCityId,
        PostalRegionId = @PostalRegionId,
        Phone = @Phone,
        Email = @Email,
        NonProfit = @NonProfit,
        FederalFundsDenied = @FederalFundsDenied,
        StateFundsDenied = @StateFundsDenied,
        OrganizedAthleticPrograms = @OrganizedAthleticPrograms,
        RejectionJustification = @RejectionJustification,
        ImageURL = @ImageURL,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 