-- Procedimiento para obtener todas las agencias con las nuevas propiedades
CREATE OR ALTER PROCEDURE [106_GetAgencies]
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
    DECLARE @TotalCount INT;

    -- Obtener los registros paginados
    SELECT DISTINCT
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

        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,
        aua.IsOwner,

        -- Datos del usuario de la agencia
        u2.Id as UserId,
        u2.FirstName AS UserFirstName,
        u2.FatherLastName AS UserFatherLastName,

        -- Datos del usuario monitor
        aua.UserId as MonitorId,
        u.FirstName AS MonitorFirstName,
        u.FatherLastName AS MonitorFatherLastName,

        -- Comentarios de la asignación de programa
        ap.Comments as ProgramRejectionJustification,
        ap.AppointmentCoordinated AS ProgramAppointmentCoordinated,
        ap.AppointmentDate AS ProgramAppointmentDate

    FROM Agency a
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId AND ap.IsActive = 1
    LEFT JOIN AgencyUsers aua ON a.Id = aua.AgencyId AND aua.IsActive = 1
    LEFT JOIN AspNetUsers u ON aua.UserId = u.Id
    LEFT JOIN AspNetUsers u2 ON a.Id = u2.AgencyId
    WHERE (@alls = 1)
        OR
         (@name IS NULL OR a.Name LIKE '%' + @name + '%')
         AND (@regionId IS NULL OR a.RegionId = @regionId)
         AND (@cityId IS NULL OR a.CityId = @cityId)
         AND (@programId IS NULL OR ap.ProgramId = @programId)
         AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
         AND (@userId IS NULL OR aua.UserId = @userId)
         AND a.IsPropietary = 0

    GROUP BY 
        a.Id, a.Name, a.AgencyStatusId, ast.Name, a.SdrNumber, a.UieNumber, a.EinNumber,
        a.Address, a.ZipCode, a.CityId, c.Name, a.RegionId, r.Name,
        a.PostalAddress, a.PostalZipCode, a.PostalCityId, pc.Name, a.PostalRegionId, pr.Name,
        a.Latitude, a.Longitude, a.Phone, a.Email, a.ImageURL,
        a.NonProfit, a.FederalFundsDenied, a.StateFundsDenied, a.OrganizedAthleticPrograms, a.AtRiskService,
        a.ServiceTime, a.TaxExemptionStatus, a.TaxExemptionType,
        a.IsActive, a.IsListable, a.CreatedAt, a.UpdatedAt, a.AgencyCode,
        aua.UserId, u.FirstName, u.MiddleName, u.FatherLastName, u.MotherLastName, u.Email,
        u2.Id, u2.FirstName, u2.FatherLastName, aua.IsOwner, ap.Comments, ap.AppointmentCoordinated, ap.AppointmentDate
    ORDER BY a.Name
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;

    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId AND ap.IsActive = 1
    LEFT JOIN AgencyUsers aua ON a.Id = aua.AgencyId AND aua.IsActive = 1
    WHERE (@alls = 1)
        OR (a.IsActive = 1)
        AND (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@programId IS NULL OR ap.ProgramId = @programId)
        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
        AND (@userId IS NULL OR aua.UserId = @userId)
        AND (@alls = 1 OR a.IsListable = 1);
END;
GO 