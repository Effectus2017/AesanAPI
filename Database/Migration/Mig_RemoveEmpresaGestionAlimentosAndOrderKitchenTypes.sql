-- =============================================
-- Migración: Eliminar Empresa de Gestión de Alimentos y ordenar Tipos de Cocina alfabéticamente
-- Fecha: 2025-02-12
-- Descripción: Quita la opción "Empresa de Gestión de Alimentos" (duplicada con CGA)
--              y ordena los KitchenType por DisplayOrder alfabético por Name.
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'KitchenType')
BEGIN
    PRINT 'Tabla KitchenType no existe. Saltando migración.';
    RETURN;
END

BEGIN TRY
    DECLARE @KitchenTypeEmpresa INT = (SELECT Id FROM KitchenType WHERE Name = N'Empresa de Gestión de Alimentos');
    DECLARE @KitchenTypeEmpresaAlternative INT = (SELECT Id FROM KitchenType WHERE Name LIKE N'%(EGA)%Empresa de Gestión de Alimentos%');
    DECLARE @KitchenTypeCGA INT = (SELECT Id FROM KitchenType WHERE Name = N'(CGA) Compañía de Gestión de Alimentos');

    -- 1. Eliminar "Empresa de Gestión de Alimentos" si existe (variantes: exacta o con prefijo EGA)
    IF @KitchenTypeEmpresa IS NOT NULL OR @KitchenTypeEmpresaAlternative IS NOT NULL
    BEGIN
        DECLARE @ToDelete INT = ISNULL(@KitchenTypeEmpresa, @KitchenTypeEmpresaAlternative);

        -- Eliminar de KitchenTypeGroupType
        DELETE FROM KitchenTypeGroupType WHERE KitchenTypeId = @ToDelete;

        -- Eliminar de KitchenTypeProgram
        DELETE FROM KitchenTypeProgram WHERE KitchenTypeId = @ToDelete;

        -- Eliminar de KitchenType
        DELETE FROM KitchenType WHERE Id = @ToDelete;

        PRINT 'Migración 120: Empresa de Gestión de Alimentos eliminada de KitchenType.';
    END
    ELSE
    BEGIN
        PRINT 'Migración 120: Empresa de Gestión de Alimentos no encontrada. Continuando con orden alfabético.';
    END

    -- 2. Orden alfabético por Name:
    --    (CC) Cocina Central-Solo Satélites -> 10
    --    (CCC) Cocina Central Combinada... -> 20
    --    (CGA) Compañía de Gestión de Alimentos -> 30
    --    N/A -> 40
    --    (PSS) Preparadas y Servidas en el Sitio... -> 50

    UPDATE KitchenType SET DisplayOrder = 10 WHERE Name LIKE N'(CC)%' AND Name LIKE N'%Cocina Central%';
    UPDATE KitchenType SET DisplayOrder = 20 WHERE Name LIKE N'(CCC)%' AND Name LIKE N'%Cocina Central Combinada%';
    UPDATE KitchenType SET DisplayOrder = 30 WHERE Name = N'(CGA) Compañía de Gestión de Alimentos';
    UPDATE KitchenType SET DisplayOrder = 40 WHERE Name = N'N/A';
    UPDATE KitchenType SET DisplayOrder = 50 WHERE Name LIKE N'(PSS)%' AND Name LIKE N'%Preparadas y Servidas%';

    PRINT 'Migración 120: DisplayOrder de KitchenType actualizado a orden alfabético.';
END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    RAISERROR('Error en migración 120: %s', 16, 1, @ErrorMessage);
END CATCH;
