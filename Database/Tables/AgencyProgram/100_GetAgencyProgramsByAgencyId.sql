-- =============================================
-- Stored Procedure: 100_GetAgencyProgramsByAgencyId
-- Descripción: Obtiene los IDs de los programas asociados a una agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyProgramsByAgencyId]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT ProgramId 
    FROM AgencyProgram 
    WHERE AgencyId = @agencyId 
        AND IsActive = 1;
END;
GO
