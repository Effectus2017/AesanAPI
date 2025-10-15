CREATE OR ALTER PROCEDURE [dbo].[100_UpdateOptionSelection]
    @id INT,
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @optionKey NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE OptionSelection
        SET Name = @name,
            NameEN = @nameEN,
            OptionKey = @optionKey,
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
GO 