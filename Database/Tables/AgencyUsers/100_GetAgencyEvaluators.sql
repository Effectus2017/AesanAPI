-- =============================================
-- Stored Procedure: 100_GetAgencyEvaluators
-- Descripción: Obtiene los UserIds de los evaluadores asignados directamente a una agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyEvaluators]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT au.UserId
    FROM AgencyUsers au
        INNER JOIN AspNetUserRoles ur ON au.UserId = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE au.AgencyId = @agencyId
        AND au.AgencyAssignmentType LIKE 'NUTRE_%'
        AND au.IsActive = 1
        AND r.Name = 'Evaluador';
END;
GO
