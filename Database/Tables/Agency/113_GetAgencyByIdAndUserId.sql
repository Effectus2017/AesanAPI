-- =============================================
-- Stored Procedure: 113_GetAgencyByIdAndUserId
-- Fecha: 2025-01-XX
-- Descripción: Obtiene los datos de una agencia por su ID y el ID del usuario con nueva lógica de acceso.
--              Reemplaza 112_GetAgencyByIdAndUserId con verificación de acceso.
--              Verifica que el usuario tenga acceso a la agencia según nueva lógica.
-- =============================================

CREATE OR ALTER PROCEDURE [113_GetAgencyByIdAndUserId]
    @agencyId INT,
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- NUEVA LÓGICA DE ACCESO: Verificar acceso del usuario
    -- =============================================
    DECLARE @userRoleName NVARCHAR(256) = NULL;
    DECLARE @canSeeAllAgencies BIT = 0;
    DECLARE @hasAccess BIT = 0;

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
            AND au.AgencyId = @agencyId 
            AND au.IsActive = 1
        )
        BEGIN
            SET @hasAccess = 1;
        END
    END

    -- Si no tiene acceso, retornar error
    IF @hasAccess = 0
    BEGIN
        RAISERROR('El usuario no tiene acceso a esta agencia.', 16, 1);
        RETURN -1;
    END

    -- Continuar con la consulta original (similar a 112_GetAgencyByIdAndUserId pero usando AgencyAssignmentType)
    -- Primera consulta: Obtener los datos de las agencias asignadas al usuario con datos completos de inscripción
    SELECT
        a.*,
        -- Datos del usuario de la agencia (auspiciador) - desde Staff
        s_sponsor.Id as UserId,
        s_sponsor.FirstName AS UserFirstName,
        s_sponsor.MiddleName AS UserMiddleName,
        s_sponsor.FatherLastName AS UserFatherLastName,
        s_sponsor.MotherLastName AS UserMotherLastName,
        os_position_sponsor.Name AS UserAdministrationTitle,
        -- Datos del usuario monitor (el usuario actual) - desde Staff
        s_monitor.Id as MonitorId,
        s_monitor.FirstName AS MonitorFirstName,
        s_monitor.MiddleName AS MonitorMiddleName,
        s_monitor.FatherLastName AS MonitorFatherLastName,
        s_monitor.MotherLastName AS MonitorMotherLastName,
        os_position_monitor.Name AS MonitorAdministrationTitle,
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
        ast.Name as StatusName
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
    WHERE a.Id = @agencyId;

    -- Segunda consulta: Obtener los programas asociados a las agencias del usuario
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
        INNER JOIN AgencyUsers aua ON ap.AgencyId = aua.AgencyId AND aua.IsActive = 1
    WHERE aua.UserId = @userId
        AND (@agencyId IS NULL OR ap.AgencyId = @agencyId);

    -- Tercera consulta: Obtener los usuarios que hicieron appointments en las agencias
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
        LEFT JOIN Staff s ON u.Id = s.UserId
    WHERE aua.UserId = @userId
        AND (@agencyId IS NULL OR ap.AgencyId = @agencyId);

    -- Cuarta consulta: Obtener las funciones de autoridad de la Junta de Directores (BoardExecutiveAuthority)
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
    WHERE ai.AgencyId = @agencyId
        AND aibe.IsActive = 1
        AND os.OptionKey = 'boardExecutiveAuthority'
    ORDER BY os.DisplayOrder ASC;
END;
GO
