-- Descripción: Obtiene los datos de una agencia por su ID y el ID del usuario
-- 1.1.2 - Versión actualizada con campos de contrato del Staff
CREATE OR ALTER PROCEDURE [111_GetAgencyByIdAndUserId]
    @agencyId int,
    @userId nvarchar(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de las agencias asignadas al usuario
    SELECT
        a.*,

        -- Datos del usuario de la agencia (auspiciador) - desde Staff
        s_sponsor.Id as UserId,
        s_sponsor.FirstName AS UserFirstName,
        s_sponsor.MiddleName AS UserMiddleName,
        s_sponsor.FatherLastName AS UserFatherLastName,
        s_sponsor.MotherLastName AS UserMotherLastName,
        -- AdministrationTitle reemplazado por la posición del Staff del auspiciador
        os_position_sponsor.Name AS UserAdministrationTitle,
        -- Campos de contrato del Staff del auspiciador
        s_sponsor.ContractStartDate AS UserContractStartDate,
        s_sponsor.ContractEndDate AS UserContractEndDate,

        -- Datos del usuario monitor (el usuario actual) - desde Staff
        s_monitor.Id as MonitorId,
        s_monitor.FirstName AS MonitorFirstName,
        s_monitor.MiddleName AS MonitorMiddleName,
        s_monitor.FatherLastName AS MonitorFatherLastName,
        s_monitor.MotherLastName AS MonitorMotherLastName,
        -- AdministrationTitle reemplazado por la posición del Staff del monitor
        os_position_monitor.Name AS MonitorAdministrationTitle,
        -- Campos de contrato del Staff del monitor
        s_monitor.ContractStartDate AS MonitorContractStartDate,
        s_monitor.ContractEndDate AS MonitorContractEndDate,

        -- Comentarios de la asignación de programa
        ai.Comments as Comments,
        -- Justificación de rechazo
        ai.RejectionJustification as RejectionJustification,
        -- Cita coordinada
        ai.AppointmentCoordinated AS AppointmentCoordinated,
        -- Fecha de la cita
        ai.AppointmentDate AS AppointmentDate,

        -- Deadline to complete the registration of the Sites
        ai.DeadlineToCompleteRegistration,

        -- Datos de la agencia
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.StateFundsDenied,
        ai.StateFundsDeniedReason,
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.BasicEducationRegistryId,
        ai.ServiceTime,
        ai.TaxExemptionStatusId,
        ai.TaxExemptionTypeId,
        ai.PublicAllianceContractId,
        ai.NationalYouthProgram,
        ai.IsDayCareHome,
        a.IsPropietary,
        c.Name as CityName,
        pc.Name as PostalCityName,
        r.Name as RegionName,
        pr.Name as PostalRegionName,
        ast.Name as StatusName

    FROM Agency a
        LEFT JOIN AgencyInscription ai ON a.id = ai.AgencyId
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        LEFT JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        -- Usuario sponsor (owner) de la agencia
        LEFT JOIN AgencyUsers auaSponsor ON a.Id = auaSponsor.AgencyId AND auaSponsor.IsActive = 1 AND auaSponsor.IsOwner = 1
        -- Usuario monitor de la agencia
        LEFT JOIN AgencyUsers auaMonitor ON a.Id = auaMonitor.AgencyId AND auaMonitor.IsActive = 1 AND auaMonitor.IsMonitor = 1
        -- Datos del usuario sponsor
        LEFT JOIN AspNetUsers u2 ON auaSponsor.UserId = u2.Id
        -- Datos del usuario monitor
        LEFT JOIN AspNetUsers u ON auaMonitor.UserId = u.Id
        -- LEFT JOINs con Staff para obtener las posiciones de los usuarios y datos personales
        LEFT JOIN Staff s_sponsor ON u2.Id = s_sponsor.UserId
        LEFT JOIN Staff s_monitor ON u.Id = s_monitor.UserId
        -- LEFT JOINs con OptionSelection para obtener los nombres de las posiciones
        LEFT JOIN OptionSelection os_position_sponsor ON s_sponsor.PositionId = os_position_sponsor.Id
        LEFT JOIN OptionSelection os_position_monitor ON s_monitor.PositionId = os_position_monitor.Id
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

    -- Tercera consulta: Obtener los usuarios que hicieron appointments en las agencias - desde Staff
    SELECT DISTINCT
        u.Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        ap.AgencyId
    FROM AspNetUsers u
        INNER JOIN AgencyProgram ap ON ap.UserId = u.Id AND ap.IsActive = 1
        INNER JOIN AgencyUsers aua ON ap.AgencyId = aua.AgencyId AND aua.IsActive = 1
        -- LEFT JOIN con Staff para obtener datos personales
        LEFT JOIN Staff s ON u.Id = s.UserId
    WHERE aua.UserId = @userId
        AND (@agencyId IS NULL OR ap.AgencyId = @agencyId);
END;
GO
