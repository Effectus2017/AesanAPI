-- 100_InsertKitchenType.sql
-- Inserta un nuevo tipo de cocina
-- Cumple convención: los SPs van en la raíz de la carpeta de la tabla
-- No usar subcarpeta SP
-- Última actualización: 2024-06-10

CREATE OR ALTER PROCEDURE [100_InsertKitchenType]
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO KitchenType
        (Name, NameEN, IsActive, DisplayOrder, CreatedAt)
    VALUES
        (@name, @nameEN, @isActive, @displayOrder, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 