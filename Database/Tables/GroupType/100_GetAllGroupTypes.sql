-- 100_GetAllGroupTypes.sql
-- Obtiene todos los tipos de grupo
-- Cumple convención: los SPs van en la raíz de la carpeta de la tabla
-- No usar subcarpeta SP
-- Última actualización: 2024-06-10
CREATE OR ALTER PROCEDURE [100_GetAllGroupTypes]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
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
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY DisplayOrder, Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM GroupType
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%');
END; 