-- Procedimiento para actualizar un programa existente
CREATE OR ALTER PROCEDURE [100_UpdateProgram]
    @id INT,
    @name NVARCHAR(255),
    @description NVARCHAR(MAX),
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE Program
        SET Name = @name,
            Description = @description,
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