CREATE OR ALTER PROCEDURE [dbo].[100_InsertOptionSelection]
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @optionKey NVARCHAR(255),
    @isActive BIT = 1,
    @displayOrder INT = 0,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, IsActive, DisplayOrder, CreatedAt)
    VALUES
        (@name, @nameEN, @optionKey, @isActive, @displayOrder, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO 