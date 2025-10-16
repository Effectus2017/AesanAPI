-- 100_UpdateCenterType
-- Actualiza un tipo de centro
CREATE OR ALTER PROCEDURE [100_UpdateCenterType]
    @id INT,
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @displayOrder INT,
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE CenterType
        SET Name = @name,
            NameEN = @nameEN,
            DisplayOrder = @displayOrder,
            IsActive = @isActive,
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
GO 