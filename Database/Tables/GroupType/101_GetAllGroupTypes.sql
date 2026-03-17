-- =============================================
-- Stored Procedure: 101_GetAllGroupTypes
-- Versión 2: igual que 100_ pero incluye columna Code en el resultado.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetAllGroupTypes]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = Id,
        name = Name,
        nameen = NameEN,
        code = Code,
        isactive = IsActive,
        displayorder = DisplayOrder,
        createdat = CreatedAt,
        updatedat = UpdatedAt
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
