-- Procedimiento para obtener una agencia por ID con las nuevas propiedades
CREATE OR ALTER PROCEDURE [106_GetAgencyById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

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
        
        -- Comentarios de la asignación de programa
        ap.Comments as ProgramRejectionJustification,
        ap.AppointmentCoordinated AS ProgramAppointmentCoordinated,
        ap.AppointmentDate AS ProgramAppointmentDate,

        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,

        -- Datos del usuario de la agencia (owner)
        au.UserId as UserId,
        u2.FirstName AS UserFirstName,
        u2.FatherLastName AS UserFatherLastName,
        u2.MiddleName AS UserMiddleName,
        u2.MotherLastName AS UserMotherLastName,
        u2.AdministrationTitle AS UserAdministrationTitle,
        u2.Email AS UserEmail,
        u2.ImageURL AS UserImageURL,

        -- Datos del usuario monitor
        aua.UserId as MonitorId,
        u.FirstName AS MonitorFirstName,
        u.FatherLastName AS MonitorFatherLastName,
        u.MiddleName AS MonitorMiddleName,
        u.ImageURL AS MonitorImageURL

    FROM Agency a
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    -- Obtener el usuario monitor
    LEFT JOIN AgencyUsers aua ON a.Id = aua.AgencyId AND aua.IsActive = 1 AND aua.IsMonitor = 1
    LEFT JOIN AspNetUsers u ON aua.UserId = u.Id
    -- Obtener el usuario propietario
    LEFT JOIN AgencyUsers au ON a.Id = au.AgencyId AND au.IsActive = 1 AND au.IsOwner = 1
    LEFT JOIN AspNetUsers u2 ON au.UserId = u2.Id
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId AND ap.IsActive = 1
    WHERE a.Id = @Id AND a.IsActive = 1;

    -- Obtener los programas asociados a la agencia
    SELECT
        p.Id,
        p.Name,
        p.Description,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    WHERE ap.AgencyId = @Id;
END;
GO 