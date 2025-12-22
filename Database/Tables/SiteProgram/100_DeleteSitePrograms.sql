-- =============================================
-- Stored Procedure: 100_DeleteSitePrograms
-- Descripción: Elimina todos los programas asociados a un sitio
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSitePrograms]
    @siteId INT,
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteProgram
    WHERE SiteId = @siteId;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO

