-- =============================================
-- Stored Procedure: 101_InsertGroupType
-- Versión 2: igual que 100_ pero incluye columna Code.
-- Parámetros en lowercase según convención.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_InsertGroupType]
    @name NVARCHAR(100),
    @nameen NVARCHAR(255),
    @isactive BIT,
    @displayorder INT,
    @code NVARCHAR(50) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO GroupType
        (Name, NameEN, IsActive, DisplayOrder, Code, CreatedAt)
    VALUES
        (@name, @nameen, @isactive, @displayorder, @code, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
