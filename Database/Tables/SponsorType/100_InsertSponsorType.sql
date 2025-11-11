-- 100_InsertSponsorType.sql
-- Inserta un nuevo tipo de auspiciador
CREATE OR ALTER PROCEDURE [100_InsertSponsorType]
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @selectionNotification BIT = 0,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SponsorType
        (Name, NameEN, IsActive, DisplayOrder, SelectionNotification, CreatedAt)
    VALUES
        (@name, @nameEN, @isActive, @displayOrder, @selectionNotification, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 