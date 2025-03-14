-- Procedimiento para obtener todas las agencias con las nuevas propiedades
CREATE OR ALTER PROCEDURE [105_GetAgencies]
    @Take INT = 10,
    @Skip INT = 0,
    @Name NVARCHAR(255) = NULL,
    @RegionId INT = NULL,
    @CityId INT = NULL,
    @ProgramId INT = NULL,
    @StatusId INT = NULL,
    @UserId NVARCHAR(450) = NULL,
    @Alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TotalCount INT;

    -- Obtener el total de registros
    SELECT @TotalCount = COUNT(DISTINCT a.Id)
    FROM Agency a
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId AND ap.IsActive = 1
    LEFT JOIN AgencyUserAssignment aua ON a.Id = aua.AgencyId AND aua.IsActive = 1
    WHERE a.IsActive = 1
        AND (@Name IS NULL OR a.Name LIKE '%' + @Name + '%')
        AND (@RegionId IS NULL OR a.RegionId = @RegionId)
        AND (@CityId IS NULL OR a.CityId = @CityId)
        AND (@ProgramId IS NULL OR ap.ProgramId = @ProgramId)
        AND (@StatusId IS NULL OR a.AgencyStatusId = @StatusId)
        AND (@UserId IS NULL OR aua.UserId = @UserId)
        AND (@Alls = 1 OR a.IsListable = 1);

    -- Obtener los registros paginados
    SELECT 
        a.Id,
        a.Name,
        a.AgencyStatusId,
        ast.Name AS AgencyStatusName,
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        a.Address,
        a.ZipCode,
        a.CityId,
        c.Name AS CityName,
        a.RegionId,
        r.Name AS RegionName,
        a.PostalAddress,
        a.PostalZipCode,
        a.PostalCityId,
        pc.Name AS PostalCityName,
        a.PostalRegionId,
        pr.Name AS PostalRegionName,
        a.Latitude,
        a.Longitude,
        a.Phone,
        a.Email,
        a.ImageURL,
        a.NonProfit,
        a.FederalFundsDenied,
        a.StateFundsDenied,
        a.OrganizedAthleticPrograms,
        a.AtRiskService,
        a.ServiceTime,                -- Nueva propiedad
        a.TaxExemptionStatus,         -- Nueva propiedad
        a.TaxExemptionType,           -- Nueva propiedad
        a.RejectionJustification,
        a.Comment,
        a.AppointmentCoordinated,
        a.AppointmentDate,
        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,

        -- Datos del usuario de la agencia
        u2.Id as UserId,
        u2.FirstName AS UserFirstName,
        u2.FatherLastName AS UserFatherLastName,

        -- Datos del usuario monitor
        aua.UserId as MonitorId,
        u.FirstName AS MonitorFirstName,
        u.FatherLastName AS MonitorFatherLastName,
        @TotalCount AS TotalCount
    FROM Agency a
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId AND ap.IsActive = 1
    LEFT JOIN AgencyUserAssignment aua ON a.Id = aua.AgencyId AND aua.IsActive = 1
    LEFT JOIN AspNetUsers u ON aua.UserId = u.Id
    LEFT JOIN AspNetUsers u2 ON a.Id = u2.AgencyId
    WHERE a.IsActive = 1
        AND (@Name IS NULL OR a.Name LIKE '%' + @Name + '%')
        AND (@RegionId IS NULL OR a.RegionId = @RegionId)
        AND (@CityId IS NULL OR a.CityId = @CityId)
        AND (@ProgramId IS NULL OR ap.ProgramId = @ProgramId)
        AND (@StatusId IS NULL OR a.AgencyStatusId = @StatusId)
        AND (@UserId IS NULL OR aua.UserId = @UserId)
        AND (@Alls = 1 OR a.IsListable = 1)
        AND a.Id != 1
    GROUP BY 
        a.Id, a.Name, a.AgencyStatusId, ast.Name, a.SdrNumber, a.UieNumber, a.EinNumber,
        a.Address, a.ZipCode, a.CityId, c.Name, a.RegionId, r.Name,
        a.PostalAddress, a.PostalZipCode, a.PostalCityId, pc.Name, a.PostalRegionId, pr.Name,
        a.Latitude, a.Longitude, a.Phone, a.Email, a.ImageURL,
        a.NonProfit, a.FederalFundsDenied, a.StateFundsDenied, a.OrganizedAthleticPrograms, a.AtRiskService,
        a.ServiceTime, a.TaxExemptionStatus, a.TaxExemptionType,
        a.RejectionJustification, a.Comment, a.AppointmentCoordinated, a.AppointmentDate,
        a.IsActive, a.IsListable, a.CreatedAt, a.UpdatedAt, a.AgencyCode,
        aua.UserId, u.FirstName, u.MiddleName, u.FatherLastName, u.MotherLastName, u.Email,
        u2.Id, u2.FirstName, u2.FatherLastName
    ORDER BY a.Name
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;
END;
GO 