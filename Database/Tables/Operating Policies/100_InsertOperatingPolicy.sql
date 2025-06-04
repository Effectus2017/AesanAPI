-- 100_InsertOperatingPolicy.sql
-- Inserta un nuevo tipo de política operativa
CREATE OR ALTER PROCEDURE [100_InsertOperatingPolicy]
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO OperatingPolicies
        (Name, NameEN, IsActive, DisplayOrder, CreatedAt)
    VALUES
        (@name, @nameEN, @isActive, @displayOrder, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;