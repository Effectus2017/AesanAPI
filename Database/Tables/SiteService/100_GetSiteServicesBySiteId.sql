-- =============================================
-- Stored Procedure: 100_GetSiteServicesBySiteId
-- Descripción: Obtiene los IDs y ChildGroupIds de los servicios de un sitio
-- Fecha: 2025-01-25
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteServicesBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ChildGroupId
    FROM SiteService
    WHERE SiteId = @siteId;
END;
