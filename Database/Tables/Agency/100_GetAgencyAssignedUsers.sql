-- =============================================
-- Stored Procedure: 100_GetAgencyAssignedUsers
-- Descripción: Obtiene la lista de usuarios AESAN asignados a una agencia (evaluadores, etc.).
--              Solo incluye usuarios cuyo rol tiene AssignmentCategory = NUTRE (RoleAssignmentCategory).
--              Una fila por usuario; si tiene varios roles NUTRE, se muestra uno (ordenado por r.Name).
--              Verifica acceso del usuario con la misma lógica que 113_GetAgencyByIdAndUserId.
-- =============================================

CREATE OR ALTER PROCEDURE [100_GetAgencyAssignedUsers]
    @agencyId INT,
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userRoleName NVARCHAR(256) = NULL;
    DECLARE @hasAccess BIT = 0;

    SELECT TOP 1 @userRoleName = r.Name
    FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId;

    IF @userRoleName IN ('super_administrator', 'administrator')
        SET @hasAccess = 1;
    ELSE IF EXISTS (SELECT 1 FROM AgencyUsers au WHERE au.UserId = @userId AND au.AgencyId = @agencyId AND au.IsActive = 1)
        SET @hasAccess = 1;

    IF @hasAccess = 0
    BEGIN
        RAISERROR('El usuario no tiene acceso a esta agencia.', 16, 1);
        RETURN -1;
    END

    -- Primer result set: una fila por usuario, un rol (el primero por r.Name entre los NUTRE)
    ;WITH Ranked AS (
        SELECT
            s.Id AS staffid,
            s.FirstName,
            s.MiddleName,
            s.FatherLastName,
            s.MotherLastName,
            s.Email,
            r.Name AS rolename,
            r.DisplayName AS roledisplayname,
            ROW_NUMBER() OVER (PARTITION BY u.Id ORDER BY r.Name) AS rn
        FROM AgencyUsers au
        INNER JOIN AspNetUsers u ON au.UserId = u.Id
        INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
        INNER JOIN RoleAssignmentCategory rac ON rac.RoleId = r.Id AND rac.AssignmentCategory = N'NUTRE'
        LEFT JOIN Staff s ON u.Id = s.UserId
        WHERE au.AgencyId = @agencyId
            AND au.IsActive = 1
    )
    SELECT
        id = staffid,
        firstname = FirstName,
        middlename = MiddleName,
        fatherlastname = FatherLastName,
        motherlastname = MotherLastName,
        email = Email,
        rolename = rolename,
        roledisplayname = roledisplayname
    FROM Ranked
    WHERE rn = 1
    ORDER BY FirstName, FatherLastName;

    -- Segundo result set: total de usuarios (no de filas)
    SELECT COUNT(DISTINCT au.UserId)
    FROM AgencyUsers au
    INNER JOIN AspNetUsers u ON au.UserId = u.Id
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    INNER JOIN RoleAssignmentCategory rac ON rac.RoleId = r.Id AND rac.AssignmentCategory = N'NUTRE'
    WHERE au.AgencyId = @agencyId
        AND au.IsActive = 1;
END;
GO
