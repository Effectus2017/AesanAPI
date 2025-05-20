CREATE OR ALTER PROCEDURE [dbo].[102_GetHouseholdMemberIncomes]
    @memberId INT,
    @take INT = 50,
    @skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM HouseholdMemberIncome
    WHERE MemberId = @memberId AND IsActive = 1
    ORDER BY Id DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
END; 