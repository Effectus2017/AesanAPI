CREATE OR ALTER PROCEDURE [dbo].[100_GetMealTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id,
           Name
    FROM MealType
    WHERE Id = @id;
END; 