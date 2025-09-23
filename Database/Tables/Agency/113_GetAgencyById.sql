-- Obtener una agencia por su id con result sets separados para usuarios y monitores
-- Versión actualizada con JOINs a OptionSelection para datos de inscripción
-- 1.1.3
CREATE OR ALTER PROCEDURE [113_GetAgencyById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Primer result set: Datos de la agencia con datos completos de inscripción
    SELECT
        a.Id,
        a.Name,
        a.AgencyStatusId as StatusId,
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
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.BasicEducationRegistryId,
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
        os_pac.OptionKey AS PublicAllianceContractOptionKey

    FROM Agency a
        INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        INNER JOIN City c ON a.CityId = c.Id
        INNER JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
        -- JOINs con OptionSelection para datos de inscripción
        LEFT JOIN OptionSelection os_ber ON ai.BasicEducationRegistryId = os_ber.Id
        LEFT JOIN OptionSelection os_tes ON ai.TaxExemptionStatusId = os_tes.Id
        LEFT JOIN OptionSelection os_tet ON ai.TaxExemptionTypeId = os_tet.Id
        LEFT JOIN OptionSelection os_toe ON ai.TypeOfEntityId = os_toe.Id
        LEFT JOIN OptionSelection os_toa ON ai.TypeOfApplicantId = os_toa.Id
        LEFT JOIN OptionSelection os_pac ON ai.PublicAllianceContractId = os_pac.Id
    WHERE a.Id = @id AND a.IsActive = 1;

    -- Segundo result set: Programas asociados a la agencia
    SELECT
        p.Id,
        p.Name,
        p.NameEN,
        p.Description,
        p.DescriptionEN,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    WHERE ap.AgencyId = @id AND ap.IsActive = 1;

    -- Tercer result set: Usuario monitor asociado a la agencia
    WITH
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId, CreatedAt
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1 AND AgencyId = @id
        ),
        AgencyMonitorsWithDataCTE
        AS
        (
            SELECT DISTINCT
                mon.AgencyId,
                mon.UserId,
                ROW_NUMBER() OVER (PARTITION BY mon.AgencyId ORDER BY mon.CreatedAt DESC) as rn
            FROM AgencyMonitorsCTE mon
        )
    SELECT DISTINCT
        mon.AgencyId,
        mon.UserId,
        -- Datos del usuario desde Staff
        s.Id as Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        -- Campos de posición del usuario
        s.PositionId,
        os.Name AS PositionName,
        os.NameEN AS PositionNameEN,
        os.OptionKey AS PositionOptionKey,
        -- Campos de contrato del usuario
        s.ContractStartDate,
        s.ContractEndDate,
        -- Información adicional del usuario
        s.Email,
        s.BirthDate,
        s.StatusId,
        os_status.Name AS StatusName,
        os_status.NameEN AS StatusNameEN
    FROM AgencyMonitorsWithDataCTE mon
        LEFT JOIN AspNetUsers u ON mon.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE mon.rn = 1;

    -- Cuarto result set: Usuario owner (que creó la agencia)
    WITH
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId, CreatedAt
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1 AND AgencyId = @id
        ),
        AgencyOwnersWithDataCTE
        AS
        (
            SELECT DISTINCT
                own.AgencyId,
                own.UserId,
                ROW_NUMBER() OVER (PARTITION BY own.AgencyId ORDER BY own.CreatedAt DESC) as rn
            FROM AgencyOwnersCTE own
        )
    SELECT DISTINCT
        own.AgencyId,
        own.UserId,
        -- Datos del usuario desde Staff
        s.Id as Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        -- Campos de posición del usuario
        s.PositionId,
        os.Name AS PositionName,
        os.NameEN AS PositionNameEN,
        os.OptionKey AS PositionOptionKey,
        -- Campos de contrato del usuario
        s.ContractStartDate,
        s.ContractEndDate,
        -- Información adicional del usuario
        s.Email,
        s.BirthDate,
        s.StatusId,
        os_status.Name AS StatusName,
        os_status.NameEN AS StatusNameEN
    FROM AgencyOwnersWithDataCTE own
        LEFT JOIN AspNetUsers u ON own.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE own.rn = 1;
END;
GO

-- EXEC [113_GetAgencyById] 1;
