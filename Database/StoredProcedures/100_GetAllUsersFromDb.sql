CREATE OR ALTER PROCEDURE [dbo].[100_GetAllUsersFromDb]
    @take INT,
    @skip INT,
    @name NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener los usuarios con sus roles
    SELECT 
        u.Id,
        u.UserName,
        u.Email,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        u.PhoneNumber,
        u.ImageURL,
        u.IsActive,
        u.AgencyId,
        r.Name as RoleName
    FROM AspNetUsers u
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE (@name IS NULL 
           OR u.FirstName LIKE '%' + @name + '%'
           OR u.FatherLastName LIKE '%' + @name + '%'
           OR u.Email LIKE '%' + @name + '%'
           OR u.UserName LIKE '%' + @name + '%')
    ORDER BY u.FirstName
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Obtener el conteo total
    SELECT COUNT(DISTINCT u.Id)
    FROM AspNetUsers u
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE (@name IS NULL 
           OR u.FirstName LIKE '%' + @name + '%'
           OR u.FatherLastName LIKE '%' + @name + '%'
           OR u.Email LIKE '%' + @name + '%'
           OR u.UserName LIKE '%' + @name + '%');
END; 