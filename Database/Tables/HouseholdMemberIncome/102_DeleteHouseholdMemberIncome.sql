CREATE OR ALTER PROCEDURE [dbo].[102_DeleteHouseholdMemberIncome]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMemberIncome
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END; 