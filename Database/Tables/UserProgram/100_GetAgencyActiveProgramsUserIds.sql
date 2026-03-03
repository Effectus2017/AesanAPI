-- =============================================
-- Stored Procedure: 100_GetAgencyActiveProgramsUserIds
-- Descripción: Obtiene los UserIds de los usuarios asociados a los programas activos de una agencia
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyActiveProgramsUserIds]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT up.UserId 
    FROM UserProgram up 
    WHERE up.ProgramId IN (
        SELECT ProgramId 
        FROM AgencyProgram 
        WHERE AgencyId = @agencyId 
            AND IsActive = 1
    ) AND up.IsActive = 1;
END;
GO
