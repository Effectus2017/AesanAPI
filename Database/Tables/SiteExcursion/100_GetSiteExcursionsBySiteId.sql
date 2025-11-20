-- =============================================
-- Stored Procedure: 100_GetSiteExcursionsBySiteId
-- Descripción: Obtiene todas las excursiones de un sitio
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteExcursionsBySiteId]
    @siteId INT,
    @includeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener excursiones del sitio
    SELECT
        se.Id, se.SiteId, s.Name AS SiteName, a.Name AS AgencyName, a.AgencyCode,
        se.ChildGroupId, scg.GroupName AS ChildGroupName,
        se.ActivityDescription, se.ExcursionDate, se.IsFullDay, se.IsUnforeseen,
        se.Comment, se.IsActive, se.CreatedAt, se.UpdatedAt
    FROM SiteExcursion se
        INNER JOIN Site s ON se.SiteId = s.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN SiteChildGroup scg ON se.ChildGroupId = scg.Id
    WHERE se.SiteId = @siteId
        AND (@includeInactive = 1 OR se.IsActive = 1)
    ORDER BY se.ExcursionDate DESC, se.CreatedAt DESC;

    -- Obtener servicios excluidos para cada excursión
    SELECT
        sees.Id, sees.SiteExcursionId, sees.ServiceTypeId,
        os.Name AS ServiceTypeName, os.NameEN AS ServiceTypeNameEN,
        os.DisplayOrder, sees.CreatedAt
    FROM SiteExcursionExcludedService sees
        INNER JOIN OptionSelection os ON sees.ServiceTypeId = os.Id
        INNER JOIN SiteExcursion se ON sees.SiteExcursionId = se.Id
    WHERE se.SiteId = @siteId
        AND (@includeInactive = 1 OR se.IsActive = 1)
    ORDER BY sees.SiteExcursionId, os.DisplayOrder;
END;

