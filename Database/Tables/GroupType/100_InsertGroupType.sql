-- 100_InsertGroupType.sql
-- Inserta un nuevo tipo de grupo
-- Cumple convención: los SPs van en la raíz de la carpeta de la tabla
-- No usar subcarpeta SP
-- Última actualización: 2024-06-10

CREATE OR ALTER PROCEDURE [100_InsertGroupType]
    @name NVARCHAR(100),
    @nameen NVARCHAR(255),
    @isactive BIT,
    @displayorder INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO GroupType
        (Name, NameEN, IsActive, DisplayOrder, CreatedAt)
    VALUES
        (@name, @nameen, @isactive, @displayorder, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 