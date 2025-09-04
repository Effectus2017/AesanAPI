-- =============================================
-- Stored Procedure: 108_GetAllUsersFromDb
-- =============================================
-- Obtiene todos los usuarios con paginación y filtros
-- Versión actualizada con campos de contrato del Staff

CREATE OR ALTER PROCEDURE [108_GetAllUsersFromDb]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @agencyId INT = NULL,
    @roles NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    WITH
        UserRoles
        AS
        (
            -- Subconsulta para obtener los roles sin duplicados
            SELECT ur.UserId,
                STRING_AGG(r.Name, ',') AS RoleName
            FROM AspNetUserRoles ur
                JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE @roles IS NULL OR r.Name IN (SELECT value
                FROM STRING_SPLIT(@roles, ','))
            GROUP BY ur.UserId
        ),
        UserAgencies
        AS
        (
            -- Subconsulta para manejar la relación con agencias
            SELECT DISTINCT UserId, AgencyId
            FROM AgencyUsers
            WHERE IsActive = 1
        )
    SELECT DISTINCT
        u.Id,
        u.Email,
        -- Datos personales desde Staff
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        -- AdministrationTitle reemplazado por la posición del Staff
        os_position.Name AS AdministrationTitle,
        s.PhoneNumber,
        s.ImageURL,
        u.IsActive,
        u.IsTemporalPasswordActived,
        u.EmailConfirmed,
        ur.RoleName,
        -- Campos de contrato del Staff
        s.ContractStartDate,
        s.ContractEndDate
    FROM AspNetUsers u
        LEFT JOIN UserRoles ur ON u.Id = ur.UserId
        LEFT JOIN UserAgencies ua ON u.Id = ua.UserId
        -- LEFT JOIN con Staff para obtener datos personales y posición del usuario
        LEFT JOIN Staff s ON u.Id = s.UserId
        -- LEFT JOIN con OptionSelection para obtener el nombre de la posición
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
    WHERE (@agencyId IS NULL OR ua.AgencyId = @agencyId)
        AND (@name IS NULL OR s.FirstName LIKE '%' + @name + '%' OR s.FatherLastName LIKE '%' + @name + '%')
    ORDER BY s.FirstName
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Count query
    SELECT COUNT(DISTINCT u.Id)
    FROM AspNetUsers u
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        LEFT JOIN AgencyUsers au ON u.Id = au.UserId AND au.IsActive = 1
        -- LEFT JOIN con Staff para obtener datos personales (consistente con la consulta principal)
        LEFT JOIN Staff s ON u.Id = s.UserId
        -- LEFT JOIN con OptionSelection para obtener el nombre de la posición (consistente con la consulta principal)
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
    WHERE (@agencyId IS NULL OR au.AgencyId = @agencyId)
        AND (@name IS NULL OR s.FirstName LIKE '%' + @name + '%' OR s.FatherLastName LIKE '%' + @name + '%')
        AND (@roles IS NULL OR r.Name IN (SELECT value
        FROM STRING_SPLIT(@roles, ',')));
END;
GO

