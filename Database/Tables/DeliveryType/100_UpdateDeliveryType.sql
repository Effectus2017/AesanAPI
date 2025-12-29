-- 100_UpdateDeliveryType.sql
-- Actualiza un tipo de entrega
CREATE OR ALTER PROCEDURE [100_UpdateDeliveryType]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE DeliveryType
        SET Name = @name,
            NameEN = @nameEN,
            IsActive = @isActive,
            DisplayOrder = @displayOrder,
            UpdatedAt = GETDATE()
        WHERE Id = @id;

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END; 