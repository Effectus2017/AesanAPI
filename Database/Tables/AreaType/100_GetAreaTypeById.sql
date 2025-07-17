CREATE OR ALTER PROCEDURE [dbo].[100_GetAreaTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, IsActive, DisplayOrder
    FROM AreaType
    WHERE Id = @id;
END 