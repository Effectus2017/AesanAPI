CREATE OR ALTER PROCEDURE [dbo].[100_DeleteFoodAuthority]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.FoodAuthority
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 