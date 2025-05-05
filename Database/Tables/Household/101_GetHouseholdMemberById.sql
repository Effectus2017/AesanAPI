CREATE OR ALTER PROCEDURE [dbo].[101_GetHouseholdMemberById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM HouseholdMember
    WHERE Id = @Id;
END; 