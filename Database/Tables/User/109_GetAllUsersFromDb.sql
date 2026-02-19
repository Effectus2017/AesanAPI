-- =============================================
-- Stored Procedure: 109_GetAllUsersFromDb
-- =============================================
-- Obtiene todos los usuarios con paginación y filtros
-- Parámetros:
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @name: Nombre para filtrar (busca en FirstName y FatherLastName)
--   @agencyId: ID de la agencia para filtrar
--   @roles: Roles para filtrar (separados por coma)
--   @alls: Si es true, retorna todos los usuarios sin filtros ni paginación
--   @excludeAdministrators: Si es 1, excluye usuarios con rol Administrator o Super-Administrator

CREATE OR ALTER PROCEDURE [109_GetAllUsersFromDb]
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @agencyId INT = NULL,
    @roles NVARCHAR(MAX) = NULL,
    @alls BIT = 0,
    @excludeAdministrators BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener usuarios con sus roles y datos personales
    SELECT DISTINCT
        u.Id,
        s.Id AS StaffId,
        s.Email,
        u.UserName,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        os_position.Name AS Position,
        s.PhoneNumber,
        s.ImageURL,
        u.IsActive,
        u.IsTemporalPasswordActived,
        u.EmailConfirmed,
        r.Id AS RoleId,
        r.Name AS RoleName,
        r.NormalizedName AS RoleNormalizedName,
        r.DisplayName,
        r.DisplayNameEN,
        s.ContractStartDate,
        s.ContractEndDate,
        s.AgencyId,
        (SELECT STRING_AGG(p.Name, ', ') WITHIN GROUP (ORDER BY p.Name)
         FROM UserProgram up
         INNER JOIN Program p ON up.ProgramId = p.Id
         WHERE up.UserId = u.Id AND up.IsActive = 1 AND p.IsActive = 1) AS ProgramName
    FROM AspNetUsers u
        -- JOIN con Staff para obtener datos personales
        LEFT JOIN Staff s ON u.Id = s.UserId
        -- JOIN con OptionSelection para obtener la posición
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
        -- JOIN con roles para obtener información completa del rol
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE (@alls = 1)
        OR ((@agencyId IS NULL OR s.AgencyId = @agencyId)
        AND (@name IS NULL OR s.FirstName LIKE '%' + @name + '%' OR s.FatherLastName LIKE '%' + @name + '%')
        AND (@roles IS NULL OR r.Name IN (SELECT value
        FROM STRING_SPLIT(@roles, ',')))
        AND (@excludeAdministrators = 0 OR u.Id NOT IN (SELECT ur2.UserId FROM AspNetUserRoles ur2 INNER JOIN AspNetRoles r2 ON ur2.RoleId = r2.Id WHERE r2.Name IN (N'administrator', N'super_administrator'))))
    ORDER BY s.FirstName, s.FatherLastName
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Count query para paginación
    SELECT COUNT(DISTINCT u.Id)
    FROM AspNetUsers u
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE (@alls = 1)
        OR ((@agencyId IS NULL OR s.AgencyId = @agencyId)
        AND (@name IS NULL OR s.FirstName LIKE '%' + @name + '%' OR s.FatherLastName LIKE '%' + @name + '%')
        AND (@roles IS NULL OR r.Name IN (SELECT value
        FROM STRING_SPLIT(@roles, ',')))
        AND (@excludeAdministrators = 0 OR u.Id NOT IN (SELECT ur2.UserId FROM AspNetUserRoles ur2 INNER JOIN AspNetRoles r2 ON ur2.RoleId = r2.Id WHERE r2.Name IN (N'administrator', N'super_administrator'))));
END;
GO

-- Ejemplos de uso:
-- Obtener usuarios paginados
-- EXEC [109_GetAllUsersFromDb] 10, 0, NULL, NULL, NULL, 0, 0;

-- Obtener todos los usuarios
-- EXEC [109_GetAllUsersFromDb] @alls = 1;

-- Filtrar por nombre
-- EXEC [109_GetAllUsersFromDb] 10, 0, 'Juan', NULL, NULL, 0, 0;

-- Filtrar por roles (sin Monitor; excluir administradores)
-- EXEC [109_GetAllUsersFromDb] 10, 0, NULL, NULL, 'Admin', 0, 1;
