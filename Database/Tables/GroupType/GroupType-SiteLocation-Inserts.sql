-- Insertar relaciones GroupTypeSiteLocation
-- Servicio en Camiones -> Móvil
-- Todos los demás Group Types -> Fijo
-- Versión: 1.0
-- Fecha: 2025-01-15

-- Obtener IDs de Site Location
DECLARE @SiteLocationFixed INT = (SELECT Id
FROM OptionSelection
WHERE OptionKey = 'siteLocation' AND Name = 'Fijo');
DECLARE @SiteLocationMobile INT = (SELECT Id
FROM OptionSelection
WHERE OptionKey = 'siteLocation' AND Name = 'Móvil');

-- Obtener ID de Servicio en Camiones
DECLARE @GroupTypeTruckService INT = (SELECT Id
FROM GroupType
WHERE Name = 'Servicio en Camiones' OR NameEN = 'Truck Service');

-- Insertar relaciones
-- Servicio en Camiones -> Móvil
IF @GroupTypeTruckService IS NOT NULL AND @SiteLocationMobile IS NOT NULL
BEGIN
    INSERT INTO GroupTypeSiteLocation
        (GroupTypeId, SiteLocationId)
    VALUES
        (@GroupTypeTruckService, @SiteLocationMobile);
END

-- Todos los demás Group Types -> Fijo
INSERT INTO GroupTypeSiteLocation
    (GroupTypeId, SiteLocationId)
SELECT gt.Id, @SiteLocationFixed
FROM GroupType gt
WHERE gt.Id != @GroupTypeTruckService
    AND gt.IsActive = 1;
