CREATE OR ALTER PROCEDURE [dbo].[101_DeleteHouseholdMember]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMember
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END; 