CREATE OR ALTER PROCEDURE [dbo].[100_UpdateOptionSelection]
    @id INT,
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @optionKey NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT,
    @isDefaultValue BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Si se marca como valor por defecto, desmarcar otras opciones del mismo OptionKey
        IF @isDefaultValue = 1
        BEGIN
            UPDATE OptionSelection
            SET IsDefaultValue = 0
            WHERE OptionKey = @optionKey AND Id != @id;
        END

        UPDATE OptionSelection
        SET Name = @name,
            NameEN = @nameEN,
            OptionKey = @optionKey,
            IsActive = @isActive,
            DisplayOrder = @displayOrder,
            IsDefaultValue = @isDefaultValue,
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