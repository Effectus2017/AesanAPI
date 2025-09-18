-- 100_UpdateDeliveryType.sql
-- Actualiza un tipo de entrega
CREATE OR ALTER PROCEDURE [100_UpdateDeliveryType]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @selectionNotification BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DeliveryType
    SET Name = @name,
        NameEN = @nameEN,
        IsActive = @isActive,
        DisplayOrder = @displayOrder,
        SelectionNotification = @selectionNotification,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
    RETURN @@ROWCOUNT;
END; 