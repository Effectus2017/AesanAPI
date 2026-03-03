-- =============================================
-- Stored Procedure: 100_GetActiveAgencyPrograms
-- Descripción: Obtiene los IDs de los programas activos de una agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetActiveAgencyPrograms]
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
