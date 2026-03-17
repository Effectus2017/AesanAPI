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

    SELECT id = Id,
        name = Name,
        nameen = NameEN,
        isactive = IsActive,
        displayorder = DisplayOrder,
        createdat = CreatedAt,
        updatedat = UpdatedAt
    FROM GroupType
    WHERE Id = @id;
END; 