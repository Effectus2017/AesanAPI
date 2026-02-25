-- =============================================
-- Stored Procedure: 100_GetUserRole
-- Descripción: Obtiene el nombre (clave) del rol del usuario desde AspNetUserRoles.
--              Usado por CalculateAgencyAssignmentTypeFromRole.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetUserRole]
    @userid NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        name = r.Name
    FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userid;
END
GO
