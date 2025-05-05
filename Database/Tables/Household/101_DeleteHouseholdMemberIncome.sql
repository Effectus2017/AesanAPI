CREATE OR ALTER PROCEDURE [dbo].[101_DeleteHouseholdMemberIncome]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMemberIncome
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END; 