-- Procedimiento para obtener una agencia por ID incluyendo el nuevo campo BasicEducationRegistry
CREATE OR ALTER PROCEDURE [109_GetAgencyById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

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
        ast.Name AS StatusName
    FROM Agency a
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    WHERE a.Id = @Id;

    RETURN 0;
END;
GO 