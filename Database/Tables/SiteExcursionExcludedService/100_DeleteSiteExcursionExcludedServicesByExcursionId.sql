-- =============================================
-- Stored Procedure: 100_DeleteSiteExcursionExcludedServicesByExcursionId
-- Descripción: Elimina todos los servicios excluidos de una excursión
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteExcursionExcludedServicesByExcursionId]
    @siteExcursionId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteExcursionExcludedService
    WHERE SiteExcursionId = @siteExcursionId;
END;

