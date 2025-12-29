-- Insertar relaciones entre DeliveryType y GroupType
-- Basado en las reglas de negocio para mostrar diálogo de permiso:
-- Satélite: Algunos tipos NO requieren permiso, otros SÍ
-- Comedor, Salón de Clases, Salones (PACNA): Todos requieren permiso

-- Obtener IDs de DeliveryType
DECLARE @DeliveryTypeNA INT = (SELECT Id FROM DeliveryType WHERE Name = 'N/A');
DECLARE @DeliveryTypeSitioRecoge INT = (SELECT Id FROM DeliveryType WHERE Name = 'Sitio Recoge');
DECLARE @DeliveryTypeAuspiciadorEntrega INT = (SELECT Id FROM DeliveryType WHERE Name = 'Auspiciador Entrega');
DECLARE @DeliveryTypeCentroComunal INT = (SELECT Id FROM DeliveryType WHERE Name = 'Centro Comunal');
DECLARE @DeliveryTypeDomicilio INT = (SELECT Id FROM DeliveryType WHERE Name = 'Domicilio');
DECLARE @DeliveryTypeRecogidoPadre INT = (SELECT Id FROM DeliveryType WHERE Name = 'Recogido de Padre o Encargado');
DECLARE @DeliveryTypeRecogido INT = (SELECT Id FROM DeliveryType WHERE Name = 'Recogido');
DECLARE @DeliveryTypeServiCarro INT = (SELECT Id FROM DeliveryType WHERE Name = 'Servi-Carro');
DECLARE @DeliveryTypeServiExpreso INT = (SELECT Id FROM DeliveryType WHERE Name = 'Servi-Expreso');

-- Obtener IDs de GroupType
DECLARE @GroupTypeSatelite INT = (SELECT Id FROM GroupType WHERE Name = 'Satélite');
DECLARE @GroupTypeComedor INT = (SELECT Id FROM GroupType WHERE Name = 'Comedor');
DECLARE @GroupTypeSalonClases INT = (SELECT Id FROM GroupType WHERE Name = 'Salón de Clases');
DECLARE @GroupTypeSalones INT = (SELECT Id FROM GroupType WHERE Name = 'Salones');

-- ============================================
-- SATÉLITE
-- ============================================
-- Tipos que NO requieren permiso
IF @GroupTypeSatelite IS NOT NULL
BEGIN
    -- Auspiciador Entrega - NO requiere permiso
    IF @DeliveryTypeAuspiciadorEntrega IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeAuspiciadorEntrega, @GroupTypeSatelite, 0);
    END

    -- Sitio Recoge - NO requiere permiso
    IF @DeliveryTypeSitioRecoge IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeSitioRecoge, @GroupTypeSatelite, 0);
    END

    -- Tipos que SÍ requieren permiso
    -- Centro Comunal - Requiere permiso
    IF @DeliveryTypeCentroComunal IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeCentroComunal, @GroupTypeSatelite, 1);
    END

    -- Domicilio - Requiere permiso
    IF @DeliveryTypeDomicilio IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeDomicilio, @GroupTypeSatelite, 1);
    END

    -- Recogido de Padre o Encargado - Requiere permiso
    IF @DeliveryTypeRecogidoPadre IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeRecogidoPadre, @GroupTypeSatelite, 1);
    END

    -- Servi-Carro - Requiere permiso
    IF @DeliveryTypeServiCarro IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiCarro, @GroupTypeSatelite, 1);
    END

    -- Servi-Expreso - Requiere permiso
    IF @DeliveryTypeServiExpreso IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiExpreso, @GroupTypeSatelite, 1);
    END
END

-- ============================================
-- COMEDOR
-- ============================================
-- Todos los tipos requieren permiso
IF @GroupTypeComedor IS NOT NULL
BEGIN
    -- Auspiciador Entrega
    IF @DeliveryTypeAuspiciadorEntrega IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeAuspiciadorEntrega, @GroupTypeComedor, 1);
    END

    -- Centro Comunal
    IF @DeliveryTypeCentroComunal IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeCentroComunal, @GroupTypeComedor, 1);
    END

    -- Domicilio
    IF @DeliveryTypeDomicilio IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeDomicilio, @GroupTypeComedor, 1);
    END

    -- Recogido de Padre o Encargado
    IF @DeliveryTypeRecogidoPadre IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeRecogidoPadre, @GroupTypeComedor, 1);
    END

    -- Servi-Carro
    IF @DeliveryTypeServiCarro IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiCarro, @GroupTypeComedor, 1);
    END

    -- Servi-Expreso
    IF @DeliveryTypeServiExpreso IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiExpreso, @GroupTypeComedor, 1);
    END

    -- Sitio Recoge
    IF @DeliveryTypeSitioRecoge IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeSitioRecoge, @GroupTypeComedor, 1);
    END
END

-- ============================================
-- SALÓN DE CLASES
-- ============================================
-- Todos los tipos requieren permiso
IF @GroupTypeSalonClases IS NOT NULL
BEGIN
    -- Auspiciador Entrega
    IF @DeliveryTypeAuspiciadorEntrega IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeAuspiciadorEntrega, @GroupTypeSalonClases, 1);
    END

    -- Centro Comunal
    IF @DeliveryTypeCentroComunal IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeCentroComunal, @GroupTypeSalonClases, 1);
    END

    -- Domicilio
    IF @DeliveryTypeDomicilio IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeDomicilio, @GroupTypeSalonClases, 1);
    END

    -- Recogido de Padre o Encargado
    IF @DeliveryTypeRecogidoPadre IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeRecogidoPadre, @GroupTypeSalonClases, 1);
    END

    -- Servi-Carro
    IF @DeliveryTypeServiCarro IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiCarro, @GroupTypeSalonClases, 1);
    END

    -- Servi-Expreso
    IF @DeliveryTypeServiExpreso IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiExpreso, @GroupTypeSalonClases, 1);
    END

    -- Sitio Recoge
    IF @DeliveryTypeSitioRecoge IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeSitioRecoge, @GroupTypeSalonClases, 1);
    END
END

-- ============================================
-- SALONES (PACNA)
-- ============================================
-- Todos los tipos requieren permiso
IF @GroupTypeSalones IS NOT NULL
BEGIN
    -- Auspiciador Entrega
    IF @DeliveryTypeAuspiciadorEntrega IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeAuspiciadorEntrega, @GroupTypeSalones, 1);
    END

    -- Centro Comunal
    IF @DeliveryTypeCentroComunal IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeCentroComunal, @GroupTypeSalones, 1);
    END

    -- Domicilio
    IF @DeliveryTypeDomicilio IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeDomicilio, @GroupTypeSalones, 1);
    END

    -- Recogido de Padre o Encargado
    IF @DeliveryTypeRecogidoPadre IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeRecogidoPadre, @GroupTypeSalones, 1);
    END

    -- Servi-Carro
    IF @DeliveryTypeServiCarro IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiCarro, @GroupTypeSalones, 1);
    END

    -- Servi-Expreso
    IF @DeliveryTypeServiExpreso IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeServiExpreso, @GroupTypeSalones, 1);
    END

    -- Sitio Recoge
    IF @DeliveryTypeSitioRecoge IS NOT NULL
    BEGIN
        INSERT INTO DeliveryTypeGroupType (DeliveryTypeId, GroupTypeId, RequiresPermission)
        VALUES (@DeliveryTypeSitioRecoge, @GroupTypeSalones, 1);
    END
END

