-- =============================================
-- Stored Procedure: 100_GetSiteProgramsBySiteId
-- Descripción: Obtiene todos los programas de un sitio
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteProgramsBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sp.Id,
        sp.SiteId,
        sp.ProgramId,
        sp.StartDate,
        sp.EndDate,
        sp.IsActive,
        sp.CreatedAt,
        sp.UpdatedAt,
        p.Name AS ProgramName,
        p.NameEN AS ProgramNameEN
    FROM SiteProgram sp
    INNER JOIN Program p ON sp.ProgramId = p.Id
    WHERE sp.SiteId = @siteId
        AND sp.IsActive = 1
    ORDER BY sp.StartDate DESC;
END;
GO

