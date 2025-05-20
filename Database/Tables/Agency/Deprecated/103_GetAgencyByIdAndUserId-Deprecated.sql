CREATE OR ALTER PROCEDURE [103_GetAgencyByIdAndUserId]
    @agencyId INT = NULL,
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de las agencias asignadas al usuario
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
        a.ServiceTime,
        a.TaxExemptionStatus,
        a.TaxExemptionType,
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
        u2.MiddleName AS UserMiddleName,
        u2.FatherLastName AS UserFatherLastName,
        u2.MotherLastName AS UserMotherLastName,
        u2.AdministrationTitle AS UserAdministrationTitle,
        
        -- Datos del usuario monitor (el usuario actual)
        aua.UserId as MonitorId,
        u.FirstName AS MonitorFirstName,
        u.MiddleName AS MonitorMiddleName,
        u.FatherLastName AS MonitorFatherLastName,
        u.MotherLastName AS MonitorMotherLastName,
        
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
    LEFT JOIN AgencyUsers aua ON a.Id = aua.AgencyId AND aua.IsActive = 1
    LEFT JOIN AspNetUsers u ON aua.UserId = u.Id
    LEFT JOIN AspNetUsers u2 ON a.Id = u2.AgencyId
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId AND ap.IsActive = 1
    WHERE a.IsActive = 1
        AND aua.UserId = @userId
        AND (@agencyId IS NULL OR a.Id = @agencyId)
    ORDER BY a.Name;

    -- Segunda consulta: Obtener los programas asociados a las agencias del usuario
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.Description,
        ap.AgencyId
    FROM Program p
    INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId AND ap.IsActive = 1
    INNER JOIN AgencyUsers aua ON ap.AgencyId = aua.AgencyId AND aua.IsActive = 1
    WHERE aua.UserId = @userId
        AND (@agencyId IS NULL OR ap.AgencyId = @agencyId);

    -- Tercera consulta: Obtener los usuarios que hicieron appointments en las agencias
    SELECT DISTINCT
        u.Id,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        ap.AgencyId
    FROM AspNetUsers u
    INNER JOIN AgencyProgram ap ON ap.UserId = u.Id AND ap.IsActive = 1
    INNER JOIN AgencyUsers aua ON ap.AgencyId = aua.AgencyId AND aua.IsActive = 1
    WHERE aua.UserId = @userId
        AND (@agencyId IS NULL OR ap.AgencyId = @agencyId);
END;
GO 