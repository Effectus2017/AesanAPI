-- 100_DeleteDeliveryType.sql
-- Elimina un tipo de entrega por id
CREATE OR ALTER PROCEDURE [100_DeleteDeliveryType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM DeliveryType WHERE Id = @id;
    RETURN @@ROWCOUNT;
END; 