-- =============================================
-- Stored Procedure: 110_GetAllUsersFromDb
-- =============================================
-- Nueva versión de 109: lista usuarios por tipo NUTRE o agencias, sin excluir roles.
-- @ispropietary = 1: usuarios NUTRE = tienen al menos un rol en RAC NUTRE O Staff en Agency con IsPropietary=1.
-- @ispropietary = 0: usuarios agencias = tienen al menos un rol en RAC AGENCY O Staff en Agency con IsPropietary=0/NULL.
-- @ispropietary = NULL: sin filtro (todos).
-- Parámetros y alias en lowercase (convención del proyecto).

CREATE OR ALTER PROCEDURE [110_GetAllUsersFromDb]
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @agencyid INT = NULL,
    @roles NVARCHAR(MAX) = NULL,
    @alls BIT = 0,
    @ispropietary BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH alloweduserids AS (
        SELECT Id AS userid FROM AspNetUsers WHERE @ispropietary IS NULL
        UNION
        -- Por rol (RoleAssignmentCategory)
        SELECT DISTINCT ur.UserId AS userid
        FROM AspNetUserRoles ur
        INNER JOIN RoleAssignmentCategory rac ON ur.RoleId = rac.RoleId
        WHERE @ispropietary IS NOT NULL
          AND ((@ispropietary = 1 AND rac.AssignmentCategory = N'NUTRE')
               OR (@ispropietary = 0 AND rac.AssignmentCategory = N'AGENCY'))
        UNION
        -- Por agencia (Staff.AgencyId -> Agency.IsPropietary)
        SELECT DISTINCT s.UserId AS userid
        FROM Staff s
        INNER JOIN Agency a ON s.AgencyId = a.Id
        WHERE @ispropietary IS NOT NULL
          AND ((@ispropietary = 1 AND a.IsPropietary = 1)
               OR (@ispropietary = 0 AND (a.IsPropietary = 0 OR a.IsPropietary IS NULL)))
    )
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
        -- ProgramId deduplicado: varias filas en UserProgram para el mismo programa repetían el nombre en STRING_AGG.
        (SELECT STRING_AGG(p.Name, ', ') WITHIN GROUP (ORDER BY p.Name)
         FROM (
             SELECT DISTINCT up.ProgramId
             FROM UserProgram up
             WHERE up.UserId = u.Id AND up.IsActive = 1
         ) AS progIds
         INNER JOIN Program p ON progIds.ProgramId = p.Id
         WHERE p.IsActive = 1) AS ProgramName
    FROM AspNetUsers u
        INNER JOIN alloweduserids au ON u.Id = au.userid
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
    WHERE (@alls = 1)
        OR ((@agencyid IS NULL OR s.AgencyId = @agencyid)
        AND (@name IS NULL OR s.FirstName LIKE '%' + @name + '%' OR s.FatherLastName LIKE '%' + @name + '%')
        AND (@roles IS NULL OR r.Name IN (SELECT value FROM STRING_SPLIT(@roles, ','))))
    ORDER BY s.FirstName, s.FatherLastName
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    ;WITH alloweduserids AS (
        SELECT Id AS userid FROM AspNetUsers WHERE @ispropietary IS NULL
        UNION
        SELECT DISTINCT ur.UserId AS userid
        FROM AspNetUserRoles ur
        INNER JOIN RoleAssignmentCategory rac ON ur.RoleId = rac.RoleId
        WHERE @ispropietary IS NOT NULL
          AND ((@ispropietary = 1 AND rac.AssignmentCategory = N'NUTRE')
               OR (@ispropietary = 0 AND rac.AssignmentCategory = N'AGENCY'))
        UNION
        SELECT DISTINCT s.UserId AS userid
        FROM Staff s
        INNER JOIN Agency a ON s.AgencyId = a.Id
        WHERE @ispropietary IS NOT NULL
          AND ((@ispropietary = 1 AND a.IsPropietary = 1)
               OR (@ispropietary = 0 AND (a.IsPropietary = 0 OR a.IsPropietary IS NULL)))
    )
    SELECT COUNT(DISTINCT u.Id)
    FROM AspNetUsers u
        INNER JOIN alloweduserids au ON u.Id = au.userid
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
    WHERE (@alls = 1)
        OR ((@agencyid IS NULL OR s.AgencyId = @agencyid)
        AND (@name IS NULL OR s.FirstName LIKE '%' + @name + '%' OR s.FatherLastName LIKE '%' + @name + '%')
        AND (@roles IS NULL OR r.Name IN (SELECT value FROM STRING_SPLIT(@roles, ','))));
END;
GO
