CREATE OR ALTER PROCEDURE [110_GetAgencies]
    @take INT = 10,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @alls BIT = 0
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
        ai.IsPropietary,
        c.Name as CityName,
        pc.Name as PostalCityName,
        r.Name as RegionName,
        pr.Name as PostalRegionName,
        ast.Name as StatusName,
        au.MonitorId,
        au.AssignedBy
    FROM Agency a
    LEFT JOIN AgencyInscription ai ON a.AgencyInscriptionId = ai.Id
    LEFT JOIN City c ON a.CityId = c.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    LEFT JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    LEFT JOIN AgencyUsers au ON a.Id = au.AgencyId
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId
    WHERE (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@programId IS NULL OR ap.ProgramId = @programId)
        AND (@userId IS NULL OR au.UserId = @userId)
        AND a.IsActive = 1
    ORDER BY a.Name
    OFFSET @skip ROWS
    FETCH NEXT CASE WHEN @alls = 1 THEN (SELECT COUNT(*) FROM Agency) ELSE @take END ROWS ONLY;

    -- Total count
    SELECT COUNT(*)
    FROM Agency a
    LEFT JOIN AgencyUsers au ON a.Id = au.AgencyId
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId
    WHERE (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@programId IS NULL OR ap.ProgramId = @programId)
        AND (@userId IS NULL OR au.UserId = @userId)
        AND a.IsActive = 1;
END;
GO 