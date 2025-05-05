CREATE OR ALTER PROCEDURE [dbo].[101_GetHouseholdMembers]
    @HouseholdId INT,
    @take INT = 50,
    @skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM HouseholdMember
    WHERE HouseholdId = @HouseholdId
    ORDER BY Id DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
END; 