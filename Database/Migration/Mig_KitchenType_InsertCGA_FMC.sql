-- ============================================
-- Migración: Insertar nuevo tipo de cocina CGA / FMC
-- Descripción: Añade "(CGA) Compañía de Gestión de Alimentos" = "(FMC) Food Management Company"
--              y sus relaciones con GroupType (mismas que Empresa de Gestión de Alimentos)
-- ============================================

IF NOT EXISTS (SELECT 1 FROM KitchenType WHERE Name = N'(CGA) Compañía de Gestión de Alimentos')
BEGIN
    INSERT INTO KitchenType (Name, NameEN, DisplayOrder)
    VALUES (N'(CGA) Compañía de Gestión de Alimentos', N'(FMC) Food Management Company', 40);
END

-- Relaciones KitchenTypeGroupType para CGA (mismas que Empresa de Gestión de Alimentos)
DECLARE @KitchenTypeCGA INT = (SELECT Id FROM KitchenType WHERE Name = N'(CGA) Compañía de Gestión de Alimentos');

IF @KitchenTypeCGA IS NOT NULL
BEGIN
    DECLARE @GroupTypeNA INT = (SELECT Id FROM GroupType WHERE Name = N'N/A');
    DECLARE @GroupTypeConsumeExterno INT = (SELECT Id FROM GroupType WHERE Name = N'Consume en Comedor-Grupo Externo');
    DECLARE @GroupTypeConsumeInterno INT = (SELECT Id FROM GroupType WHERE Name = N'Consume en Comedor-Grupo Interno');
    DECLARE @GroupTypeDomicilio INT = (SELECT Id FROM GroupType WHERE Name = N'Domicilio');
    DECLARE @GroupTypeNonCongregate INT = (SELECT Id FROM GroupType WHERE Name = N'Non-Congregate');
    DECLARE @GroupTypeServiExpreso INT = (SELECT Id FROM GroupType WHERE Name = N'Servi-Expreso');
    DECLARE @GroupTypeServicioCamiones INT = (SELECT Id FROM GroupType WHERE Name = N'Servicio en Camiones');

    IF @GroupTypeNA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeNA)
        INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeNA);
    IF @GroupTypeConsumeExterno IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeConsumeExterno)
        INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeConsumeExterno);
    IF @GroupTypeConsumeInterno IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeConsumeInterno)
        INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeConsumeInterno);
    IF @GroupTypeDomicilio IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeDomicilio)
        INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeDomicilio);
    IF @GroupTypeNonCongregate IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeNonCongregate)
        INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeNonCongregate);
    IF @GroupTypeServiExpreso IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeServiExpreso)
        INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeServiExpreso);
    IF @GroupTypeServicioCamiones IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeServicioCamiones)
        INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId) VALUES (@KitchenTypeCGA, @GroupTypeServicioCamiones);
END
