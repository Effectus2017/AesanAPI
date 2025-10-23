-- =============================================
-- Stored Procedure: 100_DeleteSiteServicesBySiteId
-- Descripción: Elimina todos los servicios de alimentación de un sitio
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteServicesBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteService 
    WHERE SiteId = @siteId;
END;
