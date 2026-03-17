-- =============================================
-- Stored Procedure: 101_GetGroupTypeById
-- Versión 2: igual que 100_ pero incluye columna Code en el resultado.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetGroupTypeById]
    @id INT
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
    WHERE Id = @id;
END;
