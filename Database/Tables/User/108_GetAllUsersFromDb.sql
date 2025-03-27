CREATE OR ALTER PROCEDURE [108_GetAllUsersFromDb]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @agencyId INT = NULL,
    @roles NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    WITH UserRoles AS (
        -- Subconsulta para obtener los roles sin duplicados
        SELECT ur.UserId,
               STRING_AGG(r.Name, ',') AS RoleName
        FROM AspNetUserRoles ur
        JOIN AspNetRoles r ON ur.RoleId = r.Id
        WHERE @roles IS NULL OR r.Name IN (SELECT value FROM STRING_SPLIT(@roles, ','))
        GROUP BY ur.UserId
    ),
    UserAgencies AS (
        -- Subconsulta para manejar la relación con agencias
        SELECT DISTINCT UserId, AgencyId
        FROM AgencyUsers
        WHERE IsActive = 1
    )
    SELECT DISTINCT
           u.Id,
           u.Email,
           u.FirstName,
           u.MiddleName,
           u.FatherLastName,
           u.MotherLastName,
           u.AdministrationTitle,
           u.PhoneNumber,
           u.ImageURL,
           u.IsActive,
           ur.RoleName
    FROM AspNetUsers u
    LEFT JOIN UserRoles ur ON u.Id = ur.UserId
    LEFT JOIN UserAgencies ua ON u.Id = ua.UserId
    WHERE (@agencyId IS NULL OR ua.AgencyId = @agencyId)
      AND (@name IS NULL OR u.FirstName LIKE '%' + @name + '%' OR u.FatherLastName LIKE '%' + @name + '%')
    ORDER BY u.FirstName
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Count query
    SELECT COUNT(DISTINCT u.Id)
    FROM AspNetUsers u
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    LEFT JOIN AgencyUsers au ON u.Id = au.UserId AND au.IsActive = 1
    WHERE (@agencyId IS NULL OR au.AgencyId = @agencyId)
      AND (@name IS NULL OR u.FirstName LIKE '%' + @name + '%' OR u.FatherLastName LIKE '%' + @name + '%')
      AND (@roles IS NULL OR r.Name IN (SELECT value FROM STRING_SPLIT(@roles, ',')));
END;
GO 