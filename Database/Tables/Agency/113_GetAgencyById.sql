-- Descripción: Obtiene los datos de una agencia por su ID con datos completos de inscripción
-- Versión actualizada con JOINs a OptionSelection para datos de inscripción
-- 1.1.2
CREATE OR ALTER PROCEDURE [113_GetAgencyById]
    @id int
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de la agencia con datos completos de inscripción
    SELECT
        a.*,

        -- Datos del usuario de la agencia (auspiciador) - desde Staff
        s_sponsor.Id as UserId,
        s_sponsor.FirstName AS UserFirstName,
        s_sponsor.MiddleName AS UserMiddleName,
        s_sponsor.FatherLastName AS UserFatherLastName,
        s_sponsor.MotherLastName AS UserMotherLastName,
        s_sponsor.Email AS UserEmail,
        s_sponsor.PositionId AS UserPositionId,
        os_position_sponsor.Name AS UserPositionName,
        os_position_sponsor.NameEN AS UserPositionNameEN,
        os_position_sponsor.OptionKey AS UserPositionOptionKey,
        s_sponsor.ContractStartDate AS UserContractStartDate,
        s_sponsor.ContractEndDate AS UserContractEndDate,
        u2.Id AS UserGuid,

        -- Datos del usuario monitor - desde Staff
        s_monitor.Id as MonitorId,
        s_monitor.FirstName AS MonitorFirstName,
        s_monitor.MiddleName AS MonitorMiddleName,
        s_monitor.FatherLastName AS MonitorFatherLastName,
        s_monitor.MotherLastName AS MonitorMotherLastName,
        s_monitor.Email AS MonitorEmail,
        s_monitor.PositionId AS MonitorPositionId,
        os_position_monitor.Name AS MonitorPositionName,
        os_position_monitor.NameEN AS MonitorPositionNameEN,
        os_position_monitor.OptionKey AS MonitorPositionOptionKey,
        s_monitor.ContractStartDate AS MonitorContractStartDate,
        s_monitor.ContractEndDate AS MonitorContractEndDate,

        -- Campos de AgencyInscription con IDs
        ai.Id AS AgencyInscriptionId,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.FederalFundsDeniedReason,
        ai.StateFundsDenied,
        ai.StateFundsDeniedReason,
        ai.BasicEducationRegistry,
        ai.TaxExemptionStatusId,
        ai.TaxExemptionTypeId,
        ai.TypeOfEntityId,
        ai.TypeOfApplicantId,
        ai.PublicAllianceContractId,
        ai.NationalYouthProgram,
        ai.IsDayCareHomeId,
        ai.ParticipatesInHeadStartProgramId,
        ai.ServicesOfferedSince,
        ai.RejectionJustification,
        ai.AppointmentCoordinated,
        ai.AppointmentDate,
        ai.Comments,
        ai.DeadlineToCompleteRegistration,
        ai.CompletedRegistrationDate,

        -- Datos de OptionSelection para BasicEducationRegistry
        os_ber.Name AS BasicEducationRegistryName,
        os_ber.NameEN AS BasicEducationRegistryNameEN,
        os_ber.OptionKey AS BasicEducationRegistryOptionKey,

        -- Datos de OptionSelection para TaxExemptionStatus
        os_tes.Name AS TaxExemptionStatusName,
        os_tes.NameEN AS TaxExemptionStatusNameEN,
        os_tes.OptionKey AS TaxExemptionStatusOptionKey,

        -- Datos de OptionSelection para TaxExemptionType
        os_tet.Name AS TaxExemptionTypeName,
        os_tet.NameEN AS TaxExemptionTypeNameEN,
        os_tet.OptionKey AS TaxExemptionTypeOptionKey,

        -- Datos de OptionSelection para TypeOfEntity
        os_toe.Name AS TypeOfEntityName,
        os_toe.NameEN AS TypeOfEntityNameEN,
        os_toe.OptionKey AS TypeOfEntityOptionKey,

        -- Datos de OptionSelection para TypeOfApplicant
        os_toa.Name AS TypeOfApplicantName,
        os_toa.NameEN AS TypeOfApplicantNameEN,
        os_toa.OptionKey AS TypeOfApplicantOptionKey,

        -- Datos de OptionSelection para PublicAllianceContract
        os_pac.Name AS PublicAllianceContractName,
        os_pac.NameEN AS PublicAllianceContractNameEN,
        os_pac.OptionKey AS PublicAllianceContractOptionKey,

        -- Datos de OptionSelection para IsDayCareHome
        os_idch.Name AS IsDayCareHomeName,
        os_idch.NameEN AS IsDayCareHomeNameEN,
        os_idch.OptionKey AS IsDayCareHomeOptionKey,
        os_idch.BooleanValue AS IsDayCareHomeBooleanValue,

        -- Datos de OptionSelection para ParticipatesInHeadStartProgram
        os_phsp.Name AS ParticipatesInHeadStartProgramName,
        os_phsp.NameEN AS ParticipatesInHeadStartProgramNameEN,
        os_phsp.OptionKey AS ParticipatesInHeadStartProgramOptionKey,

        -- Datos adicionales de la agencia
        a.IsPropietary,
        c.Name as CityName,
        c.Id as CityId,
        pc.Name as PostalCityName,
        pc.Id as PostalCityId,
        r.Name as RegionName,
        r.Id as RegionId,
        pr.Name as PostalRegionName,
        pr.Id as PostalRegionId,
        ast.Name as AgencyStatusName

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
        -- LEFT JOINs con Staff para obtener las posiciones de los usuarios
        LEFT JOIN Staff s_sponsor ON u2.Id = s_sponsor.UserId
        LEFT JOIN Staff s_monitor ON u.Id = s_monitor.UserId
        -- LEFT JOINs con OptionSelection para obtener los nombres de las posiciones
        LEFT JOIN OptionSelection os_position_sponsor ON s_sponsor.PositionId = os_position_sponsor.Id
        LEFT JOIN OptionSelection os_position_monitor ON s_monitor.PositionId = os_position_monitor.Id
        -- JOINs con OptionSelection para datos de inscripción
        LEFT JOIN OptionSelection os_ber ON ai.BasicEducationRegistry = os_ber.Id
        LEFT JOIN OptionSelection os_tes ON ai.TaxExemptionStatusId = os_tes.Id
        LEFT JOIN OptionSelection os_tet ON ai.TaxExemptionTypeId = os_tet.Id
        LEFT JOIN OptionSelection os_toe ON ai.TypeOfEntityId = os_toe.Id
        LEFT JOIN OptionSelection os_toa ON ai.TypeOfApplicantId = os_toa.Id
        LEFT JOIN OptionSelection os_pac ON ai.PublicAllianceContractId = os_pac.Id
        LEFT JOIN OptionSelection os_idch ON ai.IsDayCareHomeId = os_idch.Id
        LEFT JOIN OptionSelection os_phsp ON ai.ParticipatesInHeadStartProgramId = os_phsp.Id
    WHERE a.Id = @id;

    -- Segunda consulta: Obtener los programas asociados a la agencia
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.NameEN,
        p.Description,
        p.DescriptionEN,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt,
        ap.AgencyId
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId AND ap.IsActive = 1
    WHERE ap.AgencyId = @id;

    -- Tercera consulta: Obtener el usuario monitor asociado
    SELECT DISTINCT
        s.Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.Email,
        s.PositionId,
        os_position.Name AS PositionName,
        os_position.NameEN AS PositionNameEN,
        os_position.OptionKey AS PositionOptionKey,
        s.ContractStartDate,
        s.ContractEndDate,
        u.Id AS UserGuid
    FROM Staff s
        INNER JOIN AgencyUsers aua ON aua.AgencyId = @id AND aua.IsActive = 1 AND aua.IsMonitor = 1
        INNER JOIN AspNetUsers u ON aua.UserId = u.Id AND s.UserId = u.Id
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id;

    -- Cuarta consulta: Obtener el usuario owner (que creó la agencia)
    SELECT DISTINCT
        s.Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.Email,
        s.PositionId,
        os_position.Name AS PositionName,
        os_position.NameEN AS PositionNameEN,
        os_position.OptionKey AS PositionOptionKey,
        s.ContractStartDate,
        s.ContractEndDate,
        u.Id AS UserGuid
    FROM Staff s
        INNER JOIN AgencyUsers aua ON aua.AgencyId = @id AND aua.IsActive = 1 AND aua.IsOwner = 1
        INNER JOIN AspNetUsers u ON aua.UserId = u.Id AND s.UserId = u.Id
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id;
END;
GO

EXEC [113_GetAgencyById] 23;