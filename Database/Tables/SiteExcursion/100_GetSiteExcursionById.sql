-- =============================================
-- Stored Procedure: 100_GetSiteExcursionById
-- Descripción: Obtiene una excursión por su ID con toda la información relacionada
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteExcursionById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener información principal de la excursión
    SELECT
        se.Id, se.SiteId, s.Name AS SiteName, a.Name AS AgencyName, a.AgencyCode,
        se.ChildGroupId, scg.GroupName AS ChildGroupName,
        se.ActivityDescription, se.ExcursionDate, se.IsFullDay, se.IsUnforeseen,
        se.Comment, se.IsActive, se.CreatedAt, se.UpdatedAt
    FROM SiteExcursion se
        INNER JOIN Site s ON se.SiteId = s.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN SiteChildGroup scg ON se.ChildGroupId = scg.Id
    WHERE se.Id = @id;

    -- Obtener servicios excluidos
    SELECT
        sees.Id, sees.SiteExcursionId, sees.ServiceTypeId,
        os.Name AS ServiceTypeName, os.NameEN AS ServiceTypeNameEN,
        os.DisplayOrder, sees.CreatedAt
    FROM SiteExcursionExcludedService sees
        INNER JOIN OptionSelection os ON sees.ServiceTypeId = os.Id
    WHERE sees.SiteExcursionId = @id
    ORDER BY os.DisplayOrder;
END;

