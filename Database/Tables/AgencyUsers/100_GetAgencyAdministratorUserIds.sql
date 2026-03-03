-- =============================================
-- Stored Procedure: 100_GetAgencyAdministratorUserIds
-- Descripción: Obtiene los UserIds de los administradores asignados a una agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyAdministratorUserIds]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT au.UserId 
    FROM AgencyUsers au 
    WHERE au.AgencyId = @agencyId 
        AND au.AgencyAssignmentType = 'AGENCY_ADMINISTRATOR' 
        AND au.IsActive = 1;
END;
GO
