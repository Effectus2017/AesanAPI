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

    -- Si es super_administrator o administrator → ver todas las agencias (r.Name en BD)
    IF @userRoleName IN ('super_administrator', 'administrator')
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
        -- Datos básicos de Agency
        a.Id,
        a.Name,
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        a.Address,
        a.ZipCode,
        a.PostalAddress,
        a.PostalZipCode,
        a.Phone,
        a.Latitude,
        a.Longitude,
        a.Email,
        a.CreatedAt,
        a.UpdatedAt,
        a.ImageURL,
        a.AgencyCode,
        a.IsRecurrent,
        a.CityId,
        a.RegionId,
        a.PostalCityId,
        a.PostalRegionId,
        a.AgencyStatusId,
        a.IsPropietary,
        -- Datos del usuario de la agencia (auspiciador) - desde Staff (persona a cargo = PositionId)
        s_sponsor.Id as UserId,
        s_sponsor.FirstName AS UserFirstName,
        s_sponsor.MiddleName AS UserMiddleName,
        s_sponsor.FatherLastName AS UserFatherLastName,
        s_sponsor.MotherLastName AS UserMotherLastName,
        s_sponsor.PositionId AS UserPositionId,
        os_position_sponsor.Name AS UserPositionName,
        os_position_sponsor.NameEN AS UserPositionNameEN,
        os_position_sponsor.OptionKey AS UserPositionOptionKey,
        u2.Id AS UserGuid,
        s_sponsor.Email AS UserEmail,
        s_sponsor.ContractStartDate AS UserContractStartDate,
        s_sponsor.ContractEndDate AS UserContractEndDate,
        -- Campos de AgencyInscription
        ai.Id AS AgencyInscriptionId,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.FederalFundsDeniedReason,
        ai.StateFundsDenied,
        ai.StateFundsDeniedReason,
        ai.BasicEducationRegistry,
        ai.TaxExemptionStatusId,
        os_tax_status.Name AS TaxExemptionStatusName,
        os_tax_status.NameEN AS TaxExemptionStatusNameEN,
        os_tax_status.OptionKey AS TaxExemptionStatusOptionKey,
        ai.TaxExemptionTypeId,
        os_tax_type.Name AS TaxExemptionTypeName,
        os_tax_type.NameEN AS TaxExemptionTypeNameEN,
        os_tax_type.OptionKey AS TaxExemptionTypeOptionKey,
        ai.TypeOfEntityId,
        os_entity.Name AS TypeOfEntityName,
        os_entity.NameEN AS TypeOfEntityNameEN,
        os_entity.OptionKey AS TypeOfEntityOptionKey,
        ai.TypeOfApplicantId,
        os_applicant.Name AS TypeOfApplicantName,
        os_applicant.NameEN AS TypeOfApplicantNameEN,
        os_applicant.OptionKey AS TypeOfApplicantOptionKey,
        ai.PublicAllianceContractId,
        os_alliance.Name AS PublicAllianceContractName,
        os_alliance.NameEN AS PublicAllianceContractNameEN,
        os_alliance.OptionKey AS PublicAllianceContractOptionKey,
        ai.NationalYouthProgram,
        ai.IsDayCareHomeId,
        os_daycare.Name AS IsDayCareHomeName,
        os_daycare.NameEN AS IsDayCareHomeNameEN,
        os_daycare.OptionKey AS IsDayCareHomeOptionKey,
        os_daycare.BooleanValue AS IsDayCareHomeBooleanValue,
        ai.ParticipatesInHeadStartProgramId,
        os_headstart.Name AS ParticipatesInHeadStartProgramName,
        os_headstart.NameEN AS ParticipatesInHeadStartProgramNameEN,
        os_headstart.OptionKey AS ParticipatesInHeadStartProgramOptionKey,
        ai.ExtendedHours,
        ai.BoardMeetingsPerYear,
        ai.BoardMeetsRegularly,
        ai.ServicesOfferedSince,
        ai.RejectionJustification,
        ai.AppointmentCoordinated,
        ai.AppointmentDate,
        ai.Comments,
        ai.DeadlineToCompleteRegistration,
        ai.CompletedRegistrationDate,
        -- Relaciones City/Region
        CityName = c.Name,
        PostalCityName = pc.Name,
        RegionName = r.Name,
        PostalRegionName = pr.Name,
        -- AgencyStatus completo
        StatusId = a.AgencyStatusId,
        StatusName = ast.Name,
        StatusNameEN = ast.NameEN,
        StatusIsActive = ast.IsActive,
        StatusDisplayOrder = ast.DisplayOrder
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
        -- LEFT JOINs con OptionSelection para posiciones
        LEFT JOIN OptionSelection os_position_sponsor ON s_sponsor.PositionId = os_position_sponsor.Id
        LEFT JOIN OptionSelection os_position_monitor ON s_monitor.PositionId = os_position_monitor.Id
        -- LEFT JOINs con OptionSelection para campos de inscripción
        LEFT JOIN OptionSelection os_tax_status ON ai.TaxExemptionStatusId = os_tax_status.Id
        LEFT JOIN OptionSelection os_tax_type ON ai.TaxExemptionTypeId = os_tax_type.Id
        LEFT JOIN OptionSelection os_entity ON ai.TypeOfEntityId = os_entity.Id
        LEFT JOIN OptionSelection os_applicant ON ai.TypeOfApplicantId = os_applicant.Id
        LEFT JOIN OptionSelection os_alliance ON ai.PublicAllianceContractId = os_alliance.Id
        LEFT JOIN OptionSelection os_daycare ON ai.IsDayCareHomeId = os_daycare.Id
        LEFT JOIN OptionSelection os_headstart ON ai.ParticipatesInHeadStartProgramId = os_headstart.Id
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

    -- Quinta consulta: Usuarios asignados a la agencia (AgencyUsers + Staff)
    SELECT
        agencyid = au.AgencyId,
        userid = au.UserId,
        id = s.Id,
        firstname = s.FirstName,
        middlename = s.MiddleName,
        fatherlastname = s.FatherLastName,
        motherlastname = s.MotherLastName,
        positionid = s.PositionId,
        positionname = os.Name,
        positionnameen = os.NameEN,
        positionoptionkey = os.OptionKey,
        contractstartdate = s.ContractStartDate,
        contractenddate = s.ContractEndDate,
        email = s.Email,
        birthdate = s.BirthDate,
        statusid = s.StatusId,
        statusname = os_status.Name,
        statusnameen = os_status.NameEN
    FROM AgencyUsers au
        LEFT JOIN AspNetUsers u ON au.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE au.AgencyId = @agencyId
        AND au.IsActive = 1
    ORDER BY au.AgencyId, au.UserId;
END;
GO

EXEC [113_GetAgencyByIdAndUserId] @agencyId = 8, @userId = '77BC70EE-D0A8-4904-99D6-C92CB3D632DA';