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

-- Obtener IDs de GroupType
DECLARE @GroupTypeComedor INT = (SELECT Id
FROM GroupType
WHERE Name = 'Comedor');
DECLARE @GroupTypeSatelite INT = (SELECT Id
FROM GroupType
WHERE Name = 'Satélite');

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
DECLARE @KitchenTypeFoodCompany INT = (SELECT Id
FROM KitchenType
WHERE Name LIKE '%Empresa de Gestión de Alimentos%');

-- Obtener IDs adicionales de GroupType
DECLARE @GroupTypeNA INT = (SELECT Id
FROM GroupType
WHERE Name = 'N/A');
DECLARE @GroupTypeConsumeExterno INT = (SELECT Id
FROM GroupType
WHERE Name = 'Consume en Comedor-Grupo Externo');
DECLARE @GroupTypeConsumeInterno INT = (SELECT Id
FROM GroupType
WHERE Name = 'Consume en Comedor-Grupo Interno');
DECLARE @GroupTypeDomicilio INT = (SELECT Id
FROM GroupType
WHERE Name = 'Domicilio');
DECLARE @GroupTypeNonCongregate INT = (SELECT Id
FROM GroupType
WHERE Name = 'Non-Congregate');
DECLARE @GroupTypeServiExpreso INT = (SELECT Id
FROM GroupType
WHERE Name = 'Servi-Expreso/Carro');
DECLARE @GroupTypeServicioCamiones INT = (SELECT Id
FROM GroupType
WHERE Name = 'Servicio en Camiones');

-- Insertar relaciones para los otros tipos de grupo
-- N/A -> N/A y Empresa de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeNA IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeNA);
END

IF @KitchenTypeFoodCompany IS NOT NULL AND @GroupTypeNA IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeFoodCompany, @GroupTypeNA);
END

-- Consume en Comedor-Grupo Externo -> N/A y Empresa de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeConsumeExterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeConsumeExterno);
END

IF @KitchenTypeFoodCompany IS NOT NULL AND @GroupTypeConsumeExterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeFoodCompany, @GroupTypeConsumeExterno);
END

-- Consume en Comedor-Grupo Interno -> N/A y Empresa de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeConsumeInterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeConsumeInterno);
END

IF @KitchenTypeFoodCompany IS NOT NULL AND @GroupTypeConsumeInterno IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeFoodCompany, @GroupTypeConsumeInterno);
END

-- Domicilio -> N/A y Empresa de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeDomicilio);
END

IF @KitchenTypeFoodCompany IS NOT NULL AND @GroupTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeFoodCompany, @GroupTypeDomicilio);
END

-- Non-Congregate -> N/A y Empresa de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeNonCongregate IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeNonCongregate);
END

IF @KitchenTypeFoodCompany IS NOT NULL AND @GroupTypeNonCongregate IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeFoodCompany, @GroupTypeNonCongregate);
END

-- Servi-Expreso/Carro -> N/A y Empresa de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeServiExpreso IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeServiExpreso);
END

IF @KitchenTypeFoodCompany IS NOT NULL AND @GroupTypeServiExpreso IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeFoodCompany, @GroupTypeServiExpreso);
END

-- Servicio en Camiones -> N/A y Empresa de Gestión de Alimentos
IF @KitchenTypeNA IS NOT NULL AND @GroupTypeServicioCamiones IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeNA, @GroupTypeServicioCamiones);
END

IF @KitchenTypeFoodCompany IS NOT NULL AND @GroupTypeServicioCamiones IS NOT NULL
BEGIN
    INSERT INTO KitchenTypeGroupType
        (KitchenTypeId, GroupTypeId)
    VALUES
        (@KitchenTypeFoodCompany, @GroupTypeServicioCamiones);
END
