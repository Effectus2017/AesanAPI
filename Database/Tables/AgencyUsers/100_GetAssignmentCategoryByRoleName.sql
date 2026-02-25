-- =============================================
-- Stored Procedure: 100_GetAssignmentCategoryByRoleName
-- Descripción: Obtiene el AssignmentCategory del rol desde RoleAssignmentCategory.
--              Usado por CalculateAgencyAssignmentTypeFromRole.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAssignmentCategoryByRoleName]
    @rolename NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        assignmentcategory = rac.AssignmentCategory
    FROM RoleAssignmentCategory rac
    INNER JOIN AspNetRoles r ON rac.RoleId = r.Id
    WHERE r.Name = @rolename;
END
GO
