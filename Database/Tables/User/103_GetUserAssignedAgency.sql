-- =============================================
-- Author:      Darío
-- Create date: 2025-06-17
-- Description: Obtiene la agencia asignada a un usuario por su userId
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[103_GetUserAssignedAgency]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el usuario es Agency-Administrator
    DECLARE @isAgencyAdmin BIT = 0;
    SELECT @isAgencyAdmin = 1
    FROM AspNetUserRoles ur
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId AND r.Name = 'Agency-Administrator';

    IF @isAgencyAdmin = 1
    BEGIN
        -- Para Agency-Administrator, obtener la agencia donde es owner
        SELECT TOP 1
            a.Id,
            a.Name,
            a.Address,
            a.Phone,
            a.Email,
            a.IsActive,
            a.CreatedAt,
            a.UpdatedAt,
            au.IsOwner,
            au.IsMonitor
        FROM [dbo].[Agency] a
            INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
        WHERE au.UserId = @userId
            AND au.IsOwner = 1
            AND au.IsActive = 1
        ORDER BY au.CreatedAt DESC;
    END
    ELSE
    BEGIN
        -- Para otros usuarios, obtener la agencia principal (donde no es monitor)
        SELECT TOP 1
            a.Id,
            a.Name,
            a.Address,
            a.Phone,
            a.Email,
            a.IsActive,
            a.CreatedAt,
            a.UpdatedAt,
            au.IsOwner,
            au.IsMonitor
        FROM [dbo].[Agency] a
            INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
        WHERE au.UserId = @userId
            AND au.IsMonitor = 0
            AND au.IsActive = 1
        ORDER BY au.CreatedAt DESC;
    END
END
GO