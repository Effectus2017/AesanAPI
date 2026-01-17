-- =============================================
-- Stored Procedure: 114_GetAgencyById
-- Fecha: 2025-01-XX
-- Descripción: Obtiene los datos de una agencia por su ID con nueva lógica de acceso.
--              Reemplaza 113_GetAgencyById con verificación de acceso.
--              Verifica que el usuario tenga acceso a la agencia según nueva lógica.
-- =============================================

CREATE OR ALTER PROCEDURE [114_GetAgencyById]
    @id INT,
    @userId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- NUEVA LÓGICA DE ACCESO: Verificar acceso del usuario
    -- =============================================
    DECLARE @userRoleName NVARCHAR(256) = NULL;
    DECLARE @canSeeAllAgencies BIT = 0;
    DECLARE @hasAccess BIT = 0;

    -- Si se proporciona @userId, verificar acceso
    IF @userId IS NOT NULL
    BEGIN
        -- Obtener el rol del usuario
        SELECT TOP 1 @userRoleName = r.Name
        FROM AspNetUserRoles ur
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
        WHERE ur.UserId = @userId;

        -- Si es SuperAdministrator o Administrator → ver todas las agencias
        IF @userRoleName IN ('SuperAdministrator', 'Super-Administrador', 'Administrator', 'Administrador')
        BEGIN
            SET @canSeeAllAgencies = 1;
            SET @hasAccess = 1;
        END
        ELSE
        BEGIN
            -- Verificar si el usuario tiene acceso a esta agencia específica
            IF EXISTS (
                SELECT 1 FROM AgencyUsers au 
                WHERE au.UserId = @userId 
                AND au.AgencyId = @id 
                AND au.IsActive = 1
            )
            BEGIN
                SET @hasAccess = 1;
            END
        END
    END
    ELSE
    BEGIN
        -- Si no se proporciona @userId, permitir acceso (comportamiento legacy)
        SET @hasAccess = 1;
    END

    -- Si no tiene acceso, retornar error
    IF @hasAccess = 0
    BEGIN
        RAISERROR('El usuario no tiene acceso a esta agencia.', 16, 1);
        RETURN -1;
    END

    -- Continuar con la consulta original (similar a 113_GetAgencyById pero usando AgencyAssignmentType)
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
        -- Campos de AgencyInscription
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
        os_idch.Name AS IsDayCareHomeName,
        os_idch.NameEN AS IsDayCareHomeNameEN,
        os_idch.OptionKey AS IsDayCareHomeOptionKey,
        os_idch.BooleanValue AS IsDayCareHomeBooleanValue,
        ai.ParticipatesInHeadStartProgramId,
        ai.BoardMeetingsPerYear,
        ai.BoardMeetsRegularly,
        ai.ServicesOfferedSince,
        ai.RejectionJustification,
        ai.AppointmentCoordinated,
        ai.AppointmentDate,
        ai.Comments,
        ai.DeadlineToCompleteRegistration,
        ai.CompletedRegistrationDate,
        -- Datos adicionales
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
        -- Usuario sponsor (owner) de la agencia - usando AgencyAssignmentType
        LEFT JOIN AgencyUsers auaSponsor ON a.Id = auaSponsor.AgencyId 
            AND auaSponsor.IsActive = 1 
            AND auaSponsor.AgencyAssignmentType = 'AGENCY_OWNER'
        -- Usuario monitor de la agencia - usando AgencyAssignmentType
        LEFT JOIN AgencyUsers auaMonitor ON a.Id = auaMonitor.AgencyId 
            AND auaMonitor.IsActive = 1 
            AND auaMonitor.AgencyAssignmentType LIKE 'NUTRE_%'
        -- Datos del usuario sponsor
        LEFT JOIN AspNetUsers u2 ON auaSponsor.UserId = u2.Id
        -- Datos del usuario monitor
        LEFT JOIN AspNetUsers u ON auaMonitor.UserId = u.Id
        -- LEFT JOINs con Staff
        LEFT JOIN Staff s_sponsor ON u2.Id = s_sponsor.UserId
        LEFT JOIN Staff s_monitor ON u.Id = s_monitor.UserId
        -- LEFT JOINs con OptionSelection
        LEFT JOIN OptionSelection os_position_sponsor ON s_sponsor.PositionId = os_position_sponsor.Id
        LEFT JOIN OptionSelection os_position_monitor ON s_monitor.PositionId = os_position_monitor.Id
        LEFT JOIN OptionSelection os_idch ON ai.IsDayCareHomeId = os_idch.Id
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
        INNER JOIN AgencyUsers aua ON aua.AgencyId = @id 
            AND aua.IsActive = 1 
            AND aua.AgencyAssignmentType LIKE 'NUTRE_%'
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
        INNER JOIN AgencyUsers aua ON aua.AgencyId = @id 
            AND aua.IsActive = 1 
            AND aua.AgencyAssignmentType = 'AGENCY_OWNER'
        INNER JOIN AspNetUsers u ON aua.UserId = u.Id AND s.UserId = u.Id
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id;

    -- Quinta consulta: Obtener las funciones de autoridad de la Junta de Directores (BoardExecutiveAuthority)
    SELECT 
        aibe.OptionSelectionId,
        os.Id,
        os.Name,
        os.NameEN,
        os.OptionKey,
        os.BooleanValue,
        os.IsActive,
        os.DisplayOrder,
        os.IsDefaultValue
    FROM AgencyInscriptionBoardExecutiveAuthority aibe
        INNER JOIN AgencyInscription ai ON aibe.AgencyInscriptionId = ai.Id
        INNER JOIN OptionSelection os ON aibe.OptionSelectionId = os.Id
    WHERE ai.AgencyId = @id
        AND aibe.IsActive = 1
        AND os.OptionKey = 'boardExecutiveAuthority'
    ORDER BY os.DisplayOrder ASC;
END;
GO
