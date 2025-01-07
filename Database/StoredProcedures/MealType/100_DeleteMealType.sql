CREATE OR ALTER PROCEDURE [dbo].[100_DeleteMealType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @rowsAffected INT;

    DELETE FROM dbo.MealType
    WHERE Id = @id;
    
    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END; 