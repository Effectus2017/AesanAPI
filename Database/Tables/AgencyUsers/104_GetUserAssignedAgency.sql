-- =============================================
-- Stored Procedure: 104_GetUserAssignedAgency
-- Fecha: 2025-01-XX
-- Descripción: Obtiene la agencia asignada a un usuario por su userId.
--              Reemplaza 103_GetUserAssignedAgency con nueva lógica.
--              Retorna AgencyAssignmentType, RoleId y RoleName mediante JOIN.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[104_GetUserAssignedAgency]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener el rol del usuario
    DECLARE @userRoleName NVARCHAR(256);
    SELECT TOP 1 @userRoleName = r.Name
    FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId;

    -- Si el usuario es administrador/usuario de agencia o sponsor, obtener la agencia donde es owner.
    -- Incluye claves nuevas (Mig_RoleDisplayNameAndKey) y legacy por compatibilidad.
    IF @userRoleName IN ('agency_administrator', 'agency_user', 'Agency-Administrator', 'Sponsor-Administrador', 'Sponsor-User', 'Agency-User')
    BEGIN
        SELECT TOP 1
            a.Id,
            a.Name,
            a.Address,
            a.Phone,
            a.Email,
            a.IsActive,
            a.CreatedAt,
            a.UpdatedAt,
            au.AgencyAssignmentType,
            r.Id AS RoleId,  -- Desde JOIN
            r.Name AS RoleName  -- Desde JOIN
        FROM [dbo].[Agency] a
        INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
        INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
        WHERE au.UserId = @userId
            AND au.AgencyAssignmentType = 'AGENCY_OWNER'
            AND au.IsActive = 1
        ORDER BY au.CreatedAt DESC;
    END
    ELSE
    BEGIN
        -- Para otros usuarios (Coordinadores, Monitores, Evaluadores, etc.), 
        -- obtener la agencia principal (donde no es AGENCY_OWNER)
        SELECT TOP 1
            a.Id,
            a.Name,
            a.Address,
            a.Phone,
            a.Email,
            a.IsActive,
            a.CreatedAt,
            a.UpdatedAt,
            au.AgencyAssignmentType,
            r.Id AS RoleId,  -- Desde JOIN
            r.Name AS RoleName  -- Desde JOIN
        FROM [dbo].[Agency] a
        INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
        INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
        WHERE au.UserId = @userId
            AND au.AgencyAssignmentType != 'AGENCY_OWNER'
            AND au.IsActive = 1
        ORDER BY au.CreatedAt DESC;
    END
END
GO
