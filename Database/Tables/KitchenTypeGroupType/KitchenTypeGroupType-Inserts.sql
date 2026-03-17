-- Insertar relaciones entre KitchenType y GroupType
-- Basado en las reglas de negocio:
-- Cocina Central (CC) -> Solo Satélite
-- Cocina Central Combinada (CCC) -> Comedor y Satélite  
-- Preparadas y Servidas en el Sitio (PSS) -> Solo Comedor

-- Obtener IDs de KitchenType
DECLARE @KitchenTypeCC INT = (SELECT Id
FROM KitchenType
WHERE Name LIKE '%(CC) Cocina Central-Solo Satélites%');
DECLARE @KitchenTypeCCC INT = (SELECT Id
FROM KitchenType
WHERE Name LIKE '%(CCC) Cocina Central Combinada%');
DECLARE @KitchenTypePSS INT = (SELECT Id
FROM KitchenType
WHERE Name LIKE '%(PSS) Preparadas y Servidas en el Sitio%');

-- Obtener IDs de GroupType (por Code)
DECLARE @GroupTypeComedor INT = (SELECT Id FROM GroupType WHERE Code = N'DINING_ROOM');
DECLARE @GroupTypeSatelite INT = (SELECT Id FROM GroupType WHERE Code = N'SATELLITE');

-- Insertar relaciones
-- Cocina Central (CC) -> Solo Satélite
IF @KitchenTypeCC IS NOT NULL AND @GroupTypeSatelite IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeCC, @GroupTypeSatelite);
END

-- Cocina Central Combinada (CCC) -> Comedor y Satélite
IF @KitchenTypeCCC IS NOT NULL AND @GroupTypeComedor IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeCCC, @GroupTypeComedor);
END

IF @KitchenTypeCCC IS NOT NULL AND @GroupTypeSatelite IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeCCC, @GroupTypeSatelite);
END

-- Preparadas y Servidas en el Sitio (PSS) -> Solo Comedor
IF @KitchenTypePSS IS NOT NULL AND @GroupTypeComedor IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypePSS, @GroupTypeComedor);
END

-- Obtener IDs adicionales de KitchenType
DECLARE @KitchenTypeNA INT = (SELECT Id
FROM KitchenType
WHERE Name = 'N/A');
DECLARE @KitchenTypeCGA INT = (SELECT Id
FROM KitchenType
WHERE Name = N'(CGA) Compañía de Gestión de Alimentos');

-- Obtener IDs adicionales de GroupType (por Code)
DECLARE @GroupTypeNA INT = (SELECT Id FROM GroupType WHERE Code = N'NA');
DECLARE @GroupTypeConsumeExterno INT = (SELECT Id FROM GroupType WHERE Code = N'DINING_ROOM_EXTERNAL');
DECLARE @GroupTypeConsumeInterno INT = (SELECT Id FROM GroupType WHERE Code = N'DINING_ROOM_INTERNAL');
DECLARE @GroupTypeDomicilio INT = (SELECT Id FROM GroupType WHERE Code = N'HOME');
DECLARE @GroupTypeNonCongregate INT = (SELECT Id FROM GroupType WHERE Code = N'NON_CONGREGATE');
DECLARE @GroupTypeServiExpreso INT = (SELECT Id FROM GroupType WHERE Code = N'EXPRESS_TRAIN_CAR');
DECLARE @GroupTypeServicioCamiones INT = (SELECT Id FROM GroupType WHERE Code = N'TRUCK_SERVICE');

-- Insertar relaciones para los otros tipos de grupo
-- N/A -> N/A y (CGA) Compañía de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeNA IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeNA);
END

-- Consume en Comedor-Grupo Externo -> N/A y CGA
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeConsumeExterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeConsumeExterno);
END

-- Consume en Comedor-Grupo Interno -> N/A y CGA
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeConsumeInterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeConsumeInterno);
END

-- Domicilio -> N/A y CGA
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeDomicilio);
END

-- Non-Congregate -> N/A y CGA
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeNonCongregate IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeNonCongregate);
END

-- Servi-Expreso/Carro -> N/A y CGA
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeServiExpreso IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeServiExpreso);
END

-- Servicio en Camiones -> N/A y CGA
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeServicioCamiones IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeServicioCamiones);
END

-- (CGA) Compañía de Gestión de Alimentos = (FMC) Food Management Company
IF @KitchenTypeCGA IS NOT NULL AND @GroupTypeNA IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeNA);
END
IF @KitchenTypeCGA IS NOT NULL AND @GroupTypeConsumeExterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeConsumeExterno);
END
IF @KitchenTypeCGA IS NOT NULL AND @GroupTypeConsumeInterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeConsumeInterno);
END
IF @KitchenTypeCGA IS NOT NULL AND @GroupTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeDomicilio);
END
IF @KitchenTypeCGA IS NOT NULL AND @GroupTypeNonCongregate IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeNonCongregate);
END
IF @KitchenTypeCGA IS NOT NULL AND @GroupTypeServiExpreso IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeServiExpreso);
END
IF @KitchenTypeCGA IS NOT NULL AND @GroupTypeServicioCamiones IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeServicioCamiones);
END
