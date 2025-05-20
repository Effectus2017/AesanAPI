-- 101_DeleteHouseholdMember.sql
-- Elimina un miembro de la familia
-- Última actualización: 2025-05-19
CREATE OR ALTER PROCEDURE [dbo].[101_DeleteHouseholdMember]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMember
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END; 