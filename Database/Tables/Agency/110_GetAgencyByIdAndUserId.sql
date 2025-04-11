
-- Descripción: Obtiene los datos de una agencia por su ID y el ID del usuario

CREATE OR ALTER PROCEDURE [110_GetAgencyByIdAndUserId]
    @agencyId int,
    @userId nvarchar(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de las agencias asignadas al usuario
    SELECT
        a.*,

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

        -- Comentarios de la asignación de programa
        ai.Comments as Comments,
        -- Justificación de rechazo
        ai.RejectionJustification as RejectionJustification,
        -- Cita coordinada
        ai.AppointmentCoordinated AS AppointmentCoordinated,
        -- Fecha de la cita
        ai.AppointmentDate AS AppointmentDate,

        -- Datos de la agencia
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
        LEFT JOIN AgencyUsers auaSponsor ON a.Id = auaSponsor.AgencyId AND auaSponsor.IsActive = 1
        LEFT JOIN AgencyUsers auaMonitor ON a.Id = auaMonitor.AgencyId AND auaMonitor.IsActive = 1
        LEFT JOIN AspNetUsers u ON auaSponsor.UserId = u.Id
        LEFT JOIN AspNetUsers u2 ON auaMonitor.UserId = u2.Id
    WHERE a.Id = @agencyId;

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