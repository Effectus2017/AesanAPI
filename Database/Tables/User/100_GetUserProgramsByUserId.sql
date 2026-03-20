-- =============================================
-- Stored Procedure: 100_GetUserProgramsByUserId
-- Descripción: Obtiene los programas asignados al usuario desde UserProgram (Admin Portal).
--              Usado para claims JWT cuando el usuario tiene programas asignados.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetUserProgramsByUserId]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        p.Id,
        p.Name,
        p.NameEN,
        p.Description,
        p.DescriptionEN,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM UserProgram up
        INNER JOIN Program p ON up.ProgramId = p.Id
    WHERE up.UserId = @userId
        AND up.IsActive = 1
        AND p.IsActive = 1
    ORDER BY p.Name;
END;
GO
