-- 100_UpdateSponsorType.sql
-- Actualiza un tipo de auspiciador
CREATE OR ALTER PROCEDURE [100_UpdateSponsorType]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    UPDATE SponsorType
    SET Name = @name,
        NameEN = @nameEN,
        IsActive = @isActive,
        DisplayOrder = @displayOrder,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END; 