-- Stored procedure para obtener Site Location por Group Type
-- Similar a 100_GetKitchenTypesByGroupType
-- Versión: 1.0
-- Fecha: 2025-01-15

CREATE OR ALTER PROCEDURE [100_GetSiteLocationByGroupType]
    @groupTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT os.Id,
        os.Name,
        os.NameEN,
        os.IsActive,
        os.DisplayOrder,
        os.CreatedAt,
        os.UpdatedAt
    FROM OptionSelection os
        INNER JOIN GroupTypeSiteLocation gtsl ON os.Id = gtsl.SiteLocationId
    WHERE gtsl.GroupTypeId = @groupTypeId
        AND os.IsActive = 1
        AND gtsl.IsActive = 1
        AND os.OptionKey = 'siteLocation'
    ORDER BY os.DisplayOrder, os.Name;
END;
GO