-- Obtener una agencia por su id
-- 1.1.1
CREATE OR ALTER PROCEDURE [111_GetAgencyById]
    @id INT
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
        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,
        a.IsPropietary,

        -- Campos de AgencyInscription
        ai.Id AS AgencyInscriptionId,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.StateFundsDenied,
        ai.StateFundsDeniedReason,
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.BasicEducationRegistry,
        ai.ServiceTime,
        ai.TaxExemptionStatusId,
        ai.TaxExemptionTypeId,
        ai.TypeOfEntityId,
        ai.TypeOfApplicantId,
        ai.PublicAllianceContractId,
        ai.NationalYouthProgram,
        ai.IsDayCareHome,
        ai.RejectionJustification,
        ai.AppointmentCoordinated,
        ai.AppointmentDate,
        ai.Comments,

        -- Deadline to complete the registration of the Sites
        ai.DeadlineToCompleteRegistration,

        -- Datos del usuario de la agencia (owner) - desde Staff
        s.Id as UserId,
        s.FirstName AS UserFirstName,
        s.MiddleName AS UserMiddleName,
        s.FatherLastName AS UserFatherLastName,
        s.MotherLastName AS UserMotherLastName,

        -- AdministrationTitle reemplazado por la posición del Staff
        os_position.Name as UserAdministrationTitle,

        -- Datos del usuario monitor - desde Staff
        s_monitor.Id as MonitorId,
        s_monitor.FirstName AS MonitorFirstName,
        s_monitor.FatherLastName AS MonitorFatherLastName

    FROM Agency a
        INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        INNER JOIN City c ON a.CityId = c.Id
        INNER JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
        LEFT JOIN AgencyUsers aua ON a.Id = aua.AgencyId AND aua.IsActive = 1 AND aua.IsMonitor = 1
        LEFT JOIN AspNetUsers u ON aua.UserId = u.Id
        LEFT JOIN AgencyUsers au ON a.Id = au.AgencyId AND au.IsActive = 1 AND au.IsOwner = 1
        LEFT JOIN AspNetUsers u2 ON au.UserId = u2.Id
        -- LEFT JOIN con Staff para obtener datos del usuario owner
        LEFT JOIN Staff s ON u2.Id = s.UserId
        -- LEFT JOIN con Staff para obtener datos del monitor
        LEFT JOIN Staff s_monitor ON u.Id = s_monitor.UserId
        -- LEFT JOIN con OptionSelection para obtener el nombre de la posición del owner
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
    WHERE a.Id = @id AND a.IsActive = 1;

    -- Obtener los programas asociados a la agencia
    SELECT
        p.Id,
        p.Name,
        p.NameEN,
        p.Description,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    WHERE ap.AgencyId = @id;
END;
GO


-- EXEC [111_GetAgencyById] 1;