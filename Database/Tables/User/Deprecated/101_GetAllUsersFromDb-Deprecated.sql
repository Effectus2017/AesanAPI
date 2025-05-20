CREATE OR ALTER PROCEDURE [101_GetAllUsersFromDb]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @agencyId INT = NULL,
    @roles NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id,
           u.Email,
           u.FirstName,
           u.MiddleName,
           u.FatherLastName,
           u.MotherLastName,
           u.AdministrationTitle,
           u.PhoneNumber,
           u.ImageURL,
           u.IsActive,
           STRING_AGG(r.Name, ',') AS RoleName
    FROM AspNetUsers u
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE (@agencyId IS NULL OR u.AgencyId = @agencyId)
      AND (@name IS NULL OR u.FirstName LIKE '%' + @name + '%' OR u.FatherLastName LIKE '%' + @name + '%')
      AND (@roles IS NULL OR r.Name IN (SELECT value FROM STRING_SPLIT(@roles, ',')))
    GROUP BY u.Id, u.Email, u.FirstName, u.MiddleName, u.FatherLastName, u.MotherLastName, u.AdministrationTitle, u.PhoneNumber, u.ImageURL, u.IsActive
    ORDER BY u.FirstName
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) FROM AspNetUsers u
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE (@agencyId IS NULL OR u.AgencyId = @agencyId)
      AND (@name IS NULL OR u.FirstName LIKE '%' + @name + '%' OR u.FatherLastName LIKE '%' + @name + '%')
      AND (@roles IS NULL OR r.Name IN (SELECT value FROM STRING_SPLIT(@roles, ',')));
END;