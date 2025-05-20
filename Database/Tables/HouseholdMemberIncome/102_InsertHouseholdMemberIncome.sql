-- 102_InsertHouseholdMemberIncome.sql
-- Inserta un ingreso de un miembro de la familia
-- Última actualización: 2025-05-19
CREATE OR ALTER PROCEDURE [dbo].[102_InsertHouseholdMemberIncome]
    @memberId INT,
    @incomeTypeId INT,
    @amount DECIMAL(10,2),
    @frequencyId INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO HouseholdMemberIncome
        (
        MemberId, IncomeTypeId, Amount, FrequencyId, IsActive, CreatedAt
        )
    VALUES
        (
            @memberId, @incomeTypeId, @amount, @frequencyId, 1, GETDATE()
    );
    SET @id = SCOPE_IDENTITY();
END; 