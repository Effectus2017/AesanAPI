CREATE OR ALTER PROCEDURE [dbo].[101_GetHouseholdMemberIncomes]
    @MemberId INT,
    @take INT = 50,
    @skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM HouseholdMemberIncome
    WHERE MemberId = @MemberId
    ORDER BY Id DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
END; 