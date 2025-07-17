CREATE OR ALTER PROCEDURE [dbo].[100_UpdateAreaType]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE AreaType
    SET Name = @name, NameEN = @nameEN, IsActive = @isActive, DisplayOrder = @displayOrder
    WHERE Id = @id;
END 