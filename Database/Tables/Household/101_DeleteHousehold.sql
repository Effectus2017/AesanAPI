CREATE OR ALTER PROCEDURE [dbo].[101_DeleteHousehold]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Household
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END; 