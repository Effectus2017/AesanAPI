CREATE OR ALTER PROCEDURE [dbo].[101_UpdateHouseholdMemberIncome]
    @Id INT,
    @IncomeType NVARCHAR(100),
    @Amount DECIMAL(18,2),
    @Frequency NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMemberIncome
    SET IncomeType = @IncomeType,
        Amount = @Amount,
        Frequency = @Frequency,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END; 