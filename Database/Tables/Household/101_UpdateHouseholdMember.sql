CREATE OR ALTER PROCEDURE [dbo].[101_UpdateHouseholdMember]
    @Id INT,
    @FullName NVARCHAR(200),
    @BirthDate DATE,
    @Relationship NVARCHAR(100),
    @Gender NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMember
    SET FullName = @FullName,
        BirthDate = @BirthDate,
        Relationship = @Relationship,
        Gender = @Gender,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END; 