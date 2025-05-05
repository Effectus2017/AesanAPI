CREATE OR ALTER PROCEDURE [dbo].[101_InsertHouseholdMember]
    @HouseholdId INT,
    @FullName NVARCHAR(200),
    @BirthDate DATE,
    @Relationship NVARCHAR(100),
    @Gender NVARCHAR(20),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO HouseholdMember (
        HouseholdId, FullName, BirthDate, Relationship, Gender, IsActive, CreatedAt
    )
    VALUES (
        @HouseholdId, @FullName, @BirthDate, @Relationship, @Gender, 1, GETDATE()
    );
    SET @Id = SCOPE_IDENTITY();
END; 