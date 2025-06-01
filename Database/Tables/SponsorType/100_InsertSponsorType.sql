-- 100_InsertSponsorType.sql
-- Inserta un nuevo tipo de auspiciador
CREATE OR ALTER PROCEDURE [100_InsertSponsorType]
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SponsorType
        (Name, NameEN, IsActive, DisplayOrder, CreatedAt)
    VALUES
        (@name, @nameEN, @isActive, @displayOrder, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 