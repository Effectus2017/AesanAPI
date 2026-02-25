-- =============================================
-- Stored Procedure: 100_GetCanBeOwnerByUserId
-- Descripción: Obtiene si el usuario puede ser owner (CanBeOwner) según su rol en RoleAssignmentCategory.
--              Usado por CalculateAgencyAssignmentTypeFromRole.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetCanBeOwnerByUserId]
    @userid NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        canbeowner = rac.CanBeOwner
    FROM RoleAssignmentCategory rac
    INNER JOIN AspNetUserRoles ur ON rac.RoleId = ur.RoleId
    WHERE ur.UserId = @userid;
END
GO
