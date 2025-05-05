CREATE OR ALTER PROCEDURE [dbo].[101_GetHouseholdMemberIncomeById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM HouseholdMemberIncome
    WHERE Id = @Id;
END; 