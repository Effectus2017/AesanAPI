-- 100_GetGroupTypeById.sql
-- Obtiene un tipo de grupo por id
-- Cumple convención: los SPs van en la raíz de la carpeta de la tabla
-- No usar subcarpeta SP
-- Última actualización: 2024-06-10
CREATE OR ALTER PROCEDURE [100_GetGroupTypeById]
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
    FROM GroupType
    WHERE Id = @id;
END; 