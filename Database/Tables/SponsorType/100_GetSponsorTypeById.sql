-- 100_GetSponsorTypeById.sql
-- Obtiene un tipo de auspiciador por id
CREATE OR ALTER PROCEDURE [100_GetSponsorTypeById]
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
    FROM SponsorType
    WHERE Id = @id;
END; 