CREATE OR ALTER PROCEDURE [dbo].[100_UpdateFoodAuthority]
    @id INT,
    @name NVARCHAR(100),
    @description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.FoodAuthority
    SET Name = @name,
        Description = @description
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 