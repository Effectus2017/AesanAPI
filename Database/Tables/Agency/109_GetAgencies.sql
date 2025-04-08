-- Procedimiento para obtener todas las agencias incluyendo el nuevo campo BasicEducationRegistry
CREATE OR ALTER PROCEDURE [109_GetAgencies]
    @Take INT = 10,
    @Skip INT = 0,
    @Name NVARCHAR(255) = NULL,
    @AgencyStatusId INT = NULL,
    @MonitorId NVARCHAR(450) = NULL,
    @AssignedBy NVARCHAR(450) = NULL,
    @Alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRecords INT;
    DECLARE @TotalPages INT;
    DECLARE @TotalFiltered INT;

    -- Obtener el total de registros
    SELECT @TotalRecords = COUNT(1)
    FROM Agency a
    WHERE a.IsActive = 1;

    -- Crear tabla temporal para almacenar los resultados filtrados
    CREATE TABLE #TempResults
    (
        Id INT,
        Name NVARCHAR(255),
        AgencyStatusId INT,
        SdrNumber INT,
        UieNumber INT,
        EinNumber INT,
        Address NVARCHAR(255),
        ZipCode NVARCHAR(20),
        Phone NVARCHAR(20),
        Email NVARCHAR(255),
        CityId INT,
        RegionId INT,
        PostalAddress NVARCHAR(255),
        PostalZipCode NVARCHAR(20),
        PostalCityId INT,
        PostalRegionId INT,
        Latitude FLOAT,
        Longitude FLOAT,
        NonProfit BIT,
        BasicEducationRegistry INT,
        FederalFundsDenied BIT,
        StateFundsDenied BIT,
        OrganizedAthleticPrograms BIT,
        AtRiskService BIT,
        ServiceTime DATETIME,
        TaxExemptionStatus INT,
        TaxExemptionType INT,
        RejectionJustification NVARCHAR(MAX),
        ImageURL NVARCHAR(MAX),
        IsListable BIT,
        AgencyCode NVARCHAR(50),
        IsActive BIT,
        CreatedAt DATETIME,
        UpdatedAt DATETIME,
        CityName NVARCHAR(255),
        RegionName NVARCHAR(255),
        PostalCityName NVARCHAR(255),
        PostalRegionName NVARCHAR(255),
        StatusName NVARCHAR(255),
        MonitorId NVARCHAR(450),
        AssignedBy NVARCHAR(450)
    );

    -- Insertar resultados filtrados en la tabla temporal
    INSERT INTO #TempResults
    SELECT 
        a.Id,
        a.Name,
        a.AgencyStatusId,
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        a.Address,
        a.ZipCode,
        a.Phone,
        a.Email,
        a.CityId,
        a.RegionId,
        a.PostalAddress,
        a.PostalZipCode,
        a.PostalCityId,
        a.PostalRegionId,
        a.Latitude,
        a.Longitude,
        a.NonProfit,
        a.BasicEducationRegistry,
        a.FederalFundsDenied,
        a.StateFundsDenied,
        a.OrganizedAthleticPrograms,
        a.AtRiskService,
        a.ServiceTime,
        a.TaxExemptionStatus,
        a.TaxExemptionType,
        a.RejectionJustification,
        a.ImageURL,
        a.IsListable,
        a.AgencyCode,
        a.IsActive,
        a.CreatedAt,
        a.UpdatedAt,
        c.Name AS CityName,
        r.Name AS RegionName,
        pc.Name AS PostalCityName,
        pr.Name AS PostalRegionName,
        ast.Name AS StatusName,
        ua.MonitorId,
        ua.AssignedBy
    FROM Agency a
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    LEFT JOIN UserAgency ua ON a.Id = ua.AgencyId
    WHERE a.IsActive = 1
        AND (@Name IS NULL OR a.Name LIKE '%' + @Name + '%')
        AND (@AgencyStatusId IS NULL OR a.AgencyStatusId = @AgencyStatusId)
        AND (@MonitorId IS NULL OR ua.MonitorId = @MonitorId)
        AND (@AssignedBy IS NULL OR ua.AssignedBy = @AssignedBy);

    -- Obtener el total de registros filtrados
    SELECT @TotalFiltered = COUNT(1) FROM #TempResults;

    -- Calcular el total de páginas
    SET @TotalPages = CEILING(CAST(@TotalFiltered AS FLOAT) / @Take);

    -- Devolver los resultados paginados
    SELECT *
    FROM #TempResults
    ORDER BY CreatedAt DESC
    OFFSET @Skip ROWS
    FETCH NEXT CASE WHEN @Alls = 1 THEN @TotalFiltered ELSE @Take END ROWS ONLY;

    -- Devolver información de paginación
    SELECT @TotalRecords AS TotalRecords,
           @TotalFiltered AS TotalFiltered,
           @TotalPages AS TotalPages;

    -- Limpiar tabla temporal
    DROP TABLE #TempResults;

    RETURN 0;
END;
GO 