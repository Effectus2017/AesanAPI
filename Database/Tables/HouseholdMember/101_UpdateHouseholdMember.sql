-- 101_UpdateHouseholdMember.sql
-- Actualiza un miembro de la familia
-- Última actualización: 2025-05-19
CREATE OR ALTER PROCEDURE [dbo].[101_UpdateHouseholdMember]
    @id INT,
    @fullName NVARCHAR(200),
    @birthDate DATE,
    @relationship NVARCHAR(100),
    @gender NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HouseholdMember
    SET FullName = @fullName,
        BirthDate = @birthDate,
        Relationship = @relationship,
        Gender = @gender,
        UpdatedAt = GETDATE()
    WHERE id = @id;
END; 