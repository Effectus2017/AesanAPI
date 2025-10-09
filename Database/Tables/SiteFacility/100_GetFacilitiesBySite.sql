-- =============================================
-- Stored Procedure: 100_GetFacilitiesBySite
-- Descripción: Obtiene las instalaciones de un sitio
-- Reemplaza: 100_GetFacilitiesBySchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetFacilitiesBySite]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sf.Id,
        sf.SiteId,
        sf.FacilityTypeId,
        ft.Name AS FacilityTypeName,
        ft.NameEN AS FacilityTypeNameEN,
        ft.OptionKey AS FacilityTypeOptionKey,
        sf.Description,
        sf.IsActive,
        sf.CreatedAt,
        sf.UpdatedAt
    FROM SiteFacility sf
        INNER JOIN OptionSelection ft ON sf.FacilityTypeId = ft.Id
    WHERE sf.SiteId = @siteId AND sf.IsActive = 1
    ORDER BY ft.Name;
END;
