-- 101_GetHouseholdMemberById.sql
-- Obtiene un miembro de la familia por id
-- Última actualización: 2025-05-19
CREATE OR ALTER PROCEDURE [dbo].[101_GetHouseholdMemberById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM HouseholdMember
    WHERE id = @id;
END; 