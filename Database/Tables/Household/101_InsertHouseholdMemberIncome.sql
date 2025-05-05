CREATE OR ALTER PROCEDURE [dbo].[101_InsertHouseholdMemberIncome]
    @MemberId INT,
    @IncomeType NVARCHAR(100),
    @Amount DECIMAL(18,2),
    @Frequency NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO HouseholdMemberIncome (
        MemberId, IncomeType, Amount, Frequency, IsActive, CreatedAt
    )
    VALUES (
        @MemberId, @IncomeType, @Amount, @Frequency, 1, GETDATE()
    );
    SET @Id = SCOPE_IDENTITY();
END; 