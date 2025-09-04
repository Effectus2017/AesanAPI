CREATE OR ALTER PROCEDURE [109_GetUserById]
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
        s.AreaCode,
        s.StaffTypeId,
        s.StatusId,
        s.PositionId,
        s.AgencyId,
        s.CreatedAt AS StaffCreatedAt,
        s.UpdatedAt AS StaffUpdatedAt,
        -- Nombre de la posición desde OptionSelection
        os_position.Name AS AdministrationTitle,
        -- Nombre del tipo de staff
        os_stafftype.Name AS StaffTypeName,
        -- Nombre del status
        os_status.Name AS StatusName,
        -- Datos de la agencia
        a.Name AS AgencyName,
        a.AgencyCode
    FROM AspNetUsers u
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
        LEFT JOIN OptionSelection os_stafftype ON s.StaffTypeId = os_stafftype.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
    WHERE u.Id = @userId;

    -- Segunda consulta: Roles del usuario como objetos completos
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
    WHERE ur.UserId = @userId;
END;
GO

EXEC [109_GetUserById] 'dd56a451-42e0-4965-aa48-2980dd310b36';