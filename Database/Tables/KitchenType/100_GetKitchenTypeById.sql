-- 100_GetKitchenTypeById.sql
-- Obtiene un tipo de cocina por id
-- Cumple convención: los SPs van en la raíz de la carpeta de la tabla
-- No usar subcarpeta SP
-- Última actualización: 2024-06-10
CREATE OR ALTER PROCEDURE [100_GetKitchenTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
        Name,
        NameEN,
        IsActive,
        CreatedAt,
        UpdatedAt,
        DisplayOrder
    FROM KitchenType
    WHERE Id = @id;
END; 