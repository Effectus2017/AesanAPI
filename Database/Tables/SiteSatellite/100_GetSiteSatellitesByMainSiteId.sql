-- =============================================
-- Stored Procedure: 106_GetSiteSatellitesByMainSiteId
-- Descripción: Obtiene todos los sitios satélites de un sitio principal específico
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[106_GetSiteSatellitesByMainSiteId]
    @mainSiteId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener todos los sitios satélites del sitio principal
    SELECT
        ss.Id,
        ss.MainSiteId,
        ss.SatelliteSiteId,
        s.Name AS SatelliteSiteName,
        s.Address AS SatelliteSiteAddress,
        s.SiteNumber AS SatelliteSiteNumber,
        s.GeneralEnrollment AS SatelliteGeneralEnrollment,
        ss.AssignmentDate,
        ss.Comment,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt
    FROM SiteSatellite ss
        LEFT JOIN Site s ON ss.SatelliteSiteId = s.Id
    WHERE ss.MainSiteId = @mainSiteId
        AND ss.IsActive = 1
        AND s.IsActive = 1
    ORDER BY s.Name;

    -- Obtener el conteo total
    SELECT COUNT(*)
    FROM SiteSatellite ss
        LEFT JOIN Site s ON ss.SatelliteSiteId = s.Id
    WHERE ss.MainSiteId = @mainSiteId
        AND ss.IsActive = 1
        AND s.IsActive = 1;
END;
GO
