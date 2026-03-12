-- =============================================
-- Stored Procedure: 100_GetSiteChildGroupIdsBySiteId
-- Descripción: Devuelve los Id de los grupos de niños de un sitio.
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteChildGroupIdsBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = Id
    FROM SiteChildGroup
    WHERE SiteId = @siteId;
END;
