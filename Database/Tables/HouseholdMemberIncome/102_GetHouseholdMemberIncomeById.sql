CREATE OR ALTER PROCEDURE [dbo].[102_GetHouseholdMemberIncomeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM HouseholdMemberIncome
    WHERE Id = @id AND IsActive = 1;
END; 