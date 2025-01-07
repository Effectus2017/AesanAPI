CREATE OR ALTER PROCEDURE [dbo].[100_GetFoodAuthorityById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name,
           Description,
           CreatedAt
    FROM dbo.FoodAuthority
    WHERE Id = @id;
END; 