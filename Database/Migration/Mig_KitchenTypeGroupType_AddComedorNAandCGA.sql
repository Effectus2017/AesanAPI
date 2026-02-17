-- ============================================
-- Migración: Añadir N/A y CGA al tipo de grupo Comedor en KitchenTypeGroupType
-- Para que el dropdown de tipos de cocina en Sitios muestre N/A, CCC, PSS y CGA cuando el tipo de grupo es Comedor.
-- Idempotente: no inserta si la fila ya existe.
-- ============================================

DECLARE @GroupTypeComedor INT = (SELECT Id FROM GroupType WHERE Name = N'Comedor');
DECLARE @KitchenTypeNA INT = (SELECT Id FROM KitchenType WHERE Name = N'N/A');
DECLARE @KitchenTypeCGA INT = (SELECT Id FROM KitchenType WHERE Name LIKE N'%(CGA)%');

-- Comedor + N/A
IF @GroupTypeComedor IS NOT NULL AND @KitchenTypeNA IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeNA AND GroupTypeId = @GroupTypeComedor)
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId)
    VALUES (@KitchenTypeNA, @GroupTypeComedor);
END

-- Comedor + CGA
IF @GroupTypeComedor IS NOT NULL AND @KitchenTypeCGA IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM KitchenTypeGroupType WHERE KitchenTypeId = @KitchenTypeCGA AND GroupTypeId = @GroupTypeComedor)
BEGIN
    INSERT INTO KitchenTypeGroupType (KitchenTypeId, GroupTypeId)
    VALUES (@KitchenTypeCGA, @GroupTypeComedor);
END

GO
