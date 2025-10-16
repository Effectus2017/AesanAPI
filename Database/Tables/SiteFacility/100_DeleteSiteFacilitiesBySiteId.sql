-- =============================================
-- Stored Procedure: 100_DeleteSiteFacilitiesBySiteId
-- Descripción: Elimina todas las instalaciones de un sitio
-- Reemplaza: 100_DeleteSchoolFacilitiesBySchoolId
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteFacilitiesBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteFacility WHERE SiteId = @siteId;

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT;
END;