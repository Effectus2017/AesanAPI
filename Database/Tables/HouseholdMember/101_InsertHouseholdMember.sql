-- 101_InsertHouseholdMember.sql
-- Inserta un miembro de la familia
-- Última actualización: 2025-05-19
CREATE OR ALTER PROCEDURE [dbo].[101_InsertHouseholdMember]
    @householdId INT,
    @fullName NVARCHAR(200),
    @birthDate DATE,
    @relationship NVARCHAR(100),
    @gender NVARCHAR(20),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO HouseholdMember
        (
        HouseholdId, FullName, BirthDate, Relationship, Gender, IsActive, CreatedAt
        )
    VALUES
        (
            @householdId, @fullName, @birthDate, @relationship, @gender, 1, GETDATE()
    );
    SET @id = SCOPE_IDENTITY();
END; 