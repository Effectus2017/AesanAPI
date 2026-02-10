-- =============================================
-- Stored Procedure: 110_GetUserById
-- Fecha: 2025-01-XX
-- Descripción: Obtiene un usuario por su ID usando la nueva lógica con AgencyAssignmentType.
--              Reemplaza 109_GetUserById con nueva lógica.
--              Usa AgencyAssignmentType en lugar de IsOwner/IsMonitor.
-- =============================================

CREATE OR ALTER PROCEDURE [110_GetUserById]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Datos del usuario
    SELECT DISTINCT
        u.Id,
        u.Email,
        u.UserName,
        u.IsActive,
        u.IsTemporalPasswordActived,
        u.EmailConfirmed,
        u.UpdatedAt,
        -- Datos de Staff (datos personales)
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.PhoneNumber,
        s.ImageURL,
        s.BirthDate,
        s.PostalAddress,
        s.CityId,
        s.RegionId,
        s.ZipCode,
        s.StaffTypeId,
        s.StatusId,
        s.PositionId,
        s.AgencyId AS StaffAgencyId,
        s.CreatedAt AS StaffCreatedAt,
        s.UpdatedAt AS StaffUpdatedAt,
        -- Nombre de la posición desde OptionSelection
        os_position.Name AS AdministrationTitle,
        -- Nombre del tipo de staff
        os_stafftype.Name AS StaffTypeName,
        -- Nombre del status
        os_status.Name AS StatusName,
        -- Datos de la agencia desde AgencyUsers (relación correcta usuario-agencia)
        a.Id AS AgencyId,
        a.Name AS AgencyName,
        a.AgencyCode,
        -- Programa asignado al usuario (uno solo, para filtrado de información)
        p.Id AS ProgramId,
        p.Name AS ProgramName
    FROM AspNetUsers u
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
        LEFT JOIN OptionSelection os_stafftype ON s.StaffTypeId = os_stafftype.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
        -- Obtener la agencia desde AgencyUsers usando la nueva lógica con AgencyAssignmentType
        LEFT JOIN (
            SELECT TOP 1
            au.AgencyId,
            au.UserId
        FROM AgencyUsers au
        WHERE au.UserId = @userId
            AND au.IsActive = 1
        ORDER BY 
            CASE WHEN au.AgencyAssignmentType = 'AGENCY_OWNER' THEN 1 ELSE 2 END,
            au.CreatedAt DESC
        ) au_filtered ON u.Id = au_filtered.UserId
        LEFT JOIN Agency a ON au_filtered.AgencyId = a.Id
        -- Programa asignado al usuario (primera fila activa en UserProgram)
        LEFT JOIN (SELECT TOP 1 UserId, ProgramId FROM UserProgram WHERE UserId = @userId AND IsActive = 1) up ON up.UserId = u.Id
        LEFT JOIN Program p ON p.Id = up.ProgramId
    WHERE u.Id = @userId;

    -- Segunda consulta: Roles del usuario como objetos completos
    -- Incluir ur.IsActive = 1 o IS NULL para roles insertados por Identity (sin IsActive)
    SELECT
        r.Id,
        r.Name,
        r.Description,
        r.NormalizedName,
        r.ConcurrencyStamp,
        r.IsActive,
        r.CreatedAt,
        r.UpdatedAt
    FROM AspNetUserRoles ur
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId
        AND (ur.IsActive = 1 OR ur.IsActive IS NULL);
END;
GO

--EXEC [110_GetUserById] 'dd56a451-42e0-4965-aa48-2980dd310b36';
