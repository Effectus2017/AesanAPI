CREATE OR ALTER PROCEDURE [110_GetAgencyById]
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.*,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.StateFundsDenied,
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.BasicEducationRegistry,
        ai.ServiceTime,
        ai.TaxExemptionStatus,
        ai.TaxExemptionType,
        a.IsPropietary,
        c.Name as CityName,
        pc.Name as PostalCityName,
        r.Name as RegionName,
        pr.Name as PostalRegionName,
        ast.Name as StatusName
    FROM Agency a
    LEFT JOIN AgencyInscription ai ON a.AgencyInscriptionId = ai.Id
    LEFT JOIN City c ON a.CityId = c.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    LEFT JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    WHERE a.Id = @Id;
END;
GO 