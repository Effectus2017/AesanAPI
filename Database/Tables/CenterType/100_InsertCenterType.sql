-- 100_InsertCenterType
-- Inserta un tipo de centro
CREATE OR ALTER PROCEDURE [100_InsertCenterType]
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @displayOrder INT = 0,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO CenterType
        (Name, NameEN, DisplayOrder, IsActive, CreatedAt)
    VALUES
        (@name, @nameEN, @displayOrder, @isActive, GETDATE());

    SET @Id = SCOPE_IDENTITY();
END;
GO 