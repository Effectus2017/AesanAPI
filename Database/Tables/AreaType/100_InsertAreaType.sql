CREATE OR ALTER PROCEDURE [dbo].[100_InsertAreaType]
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT,
    @displayOrder INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AreaType
        (Name, NameEN, IsActive, DisplayOrder)
    VALUES
        (@name, @nameEN, @isActive, @displayOrder);

    SET @id = SCOPE_IDENTITY();
END 