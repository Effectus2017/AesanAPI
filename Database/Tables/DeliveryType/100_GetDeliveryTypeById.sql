-- 100_GetDeliveryTypeById.sql
-- Obtiene un tipo de entrega por id
CREATE OR ALTER PROCEDURE [100_GetDeliveryTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, IsActive, CreatedAt, UpdatedAt, DisplayOrder
    FROM DeliveryType
    WHERE Id = @id;
END; 