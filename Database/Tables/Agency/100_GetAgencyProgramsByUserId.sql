-- =============================================
-- Stored Procedure: 100_GetAgencyProgramsByUserId
-- Descripción: Obtiene los programas de la agencia asignada al usuario (AgencyUsers).
--              Usado para claims JWT (programs / programIds) cuando el usuario
--              no tiene programas en UserProgram (ej. agency_administrator).
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyProgramsByUserId]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Programas de la(s) agencia(s) a la(s) que el usuario está asignado (AgencyUsers)
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.NameEN,
        p.Description,
        p.DescriptionEN,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM AgencyUsers au
        INNER JOIN AgencyProgram ap ON au.AgencyId = ap.AgencyId AND ap.IsActive = 1
        INNER JOIN Program p ON ap.ProgramId = p.Id AND p.IsActive = 1
    WHERE au.UserId = @userId
        AND au.IsActive = 1;
END
GO

--EXEC [100_GetAgencyProgramsByUserId] @userId = '1db1104b-6c97-4f64-93e1-929296dea7bf';