-- 100_InsertDeliveryType.sql
-- Inserta un nuevo tipo de entrega
CREATE OR ALTER PROCEDURE [100_InsertDeliveryType]
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO DeliveryType
        (Name, NameEN, IsActive, DisplayOrder, CreatedAt)
    VALUES
        (@name, @nameEN, @isActive, @displayOrder, GETDATE());
    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 