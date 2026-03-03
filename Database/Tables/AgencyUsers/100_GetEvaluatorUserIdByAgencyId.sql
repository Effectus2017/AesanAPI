-- =============================================
-- Stored Procedure: 100_GetEvaluatorUserIdByAgencyId
-- Descripción: Obtiene el UserId del monitor/evaluador principal asignado a una agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetEvaluatorUserIdByAgencyId]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 UserId 
    FROM AgencyUsers 
    WHERE AgencyId = @agencyId 
        AND AgencyAssignmentType LIKE 'NUTRE_%' 
        AND IsActive = 1;
END;
GO
