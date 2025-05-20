-- 102_UpdateHouseholdMemberIncome.sql
-- Actualiza un ingreso de un miembro de la familia
-- Última actualización: 2025-05-19
CREATE OR ALTER PROCEDURE [dbo].[102_UpdateHouseholdMemberIncome]
    @id INT,
    @incomeTypeId INT,
    @amount DECIMAL(10,2),
    @frequencyId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMemberIncome
    SET IncomeTypeId = @incomeTypeId,
        Amount = @amount,
        FrequencyId = @frequencyId,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END; 