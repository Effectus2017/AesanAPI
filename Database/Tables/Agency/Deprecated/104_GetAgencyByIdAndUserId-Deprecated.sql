 -- Descripción: Obtiene los datos de una agencia por su ID y el ID del usuario
 -- Deprecated: Se reemplaza por 110_GetAgencyByIdAndUserId
 -- No Modificar este procedimiento almacenado
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
ALTER PROCEDURE [104_GetAgencyByIdAndUserId]
    @agencyId INT = NULL,
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de las agencias asignadas al usuario
    SELECT TOP 1
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
        
        -- Datos del usuario de la agencia (auspiciador)
        auaSponsor.UserId as UserId,
        u2.FirstName AS UserFirstName,
        u2.MiddleName AS UserMiddleName,
        u2.FatherLastName AS UserFatherLastName,
        u2.MotherLastName AS UserMotherLastName,
        u2.AdministrationTitle AS UserAdministrationTitle,
        
        -- Datos del usuario monitor (el usuario actual)
        auaMonitor.UserId as MonitorId,
        u.FirstName AS MonitorFirstName,
        u.MiddleName AS MonitorMiddleName,
        u.FatherLastName AS MonitorFatherLastName,
        u.MotherLastName AS MonitorMotherLastName,
        u.AdministrationTitle AS MonitorAdministrationTitle,
        
        -- Campos de AgencyInscription
        ai.Id AS AgencyInscriptionId,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.StateFundsDenied,
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.BasicEducationRegistry,
        ai.ServiceTime,
        ai.TaxExemptionStatus,
        ai.TaxExemptionType,
        ai.RejectionJustification,
        ai.AppointmentCoordinated,
        ai.AppointmentDate,
        ai.Comments

    FROM Agency a
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    -- Join para el monitor (usuario actual)
    LEFT JOIN AgencyUsers auaMonitor ON a.Id = auaMonitor.AgencyId AND auaMonitor.IsActive = 1 AND auaMonitor.UserId = @userId
    LEFT JOIN AspNetUsers u ON auaMonitor.UserId = u.Id
    -- Join para el auspiciador (solo uno)
    LEFT JOIN (
        SELECT TOP 1 * FROM AgencyUsers WHERE IsActive = 1
    ) auaSponsor ON a.Id = auaSponsor.AgencyId
    LEFT JOIN AspNetUsers u2 ON auaSponsor.UserId = u2.Id
    LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId AND ap.IsActive = 1
    LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
    WHERE a.IsActive = 1
        AND auaMonitor.UserId = @userId
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

EXEC [104_GetAgencyByIdAndUserId] 4, 'B03615AF-97D1-47D5-8A60-72AAF0A39ACF';