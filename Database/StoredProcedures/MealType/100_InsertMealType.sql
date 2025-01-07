CREATE OR ALTER PROCEDURE [dbo].[100_InsertMealType]
    @name NVARCHAR(100),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO MealType (Name)
    VALUES (@name);
    
    SET @id = SCOPE_IDENTITY();
    RETURN @id;

END; 