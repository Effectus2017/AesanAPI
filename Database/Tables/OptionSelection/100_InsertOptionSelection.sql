CREATE OR ALTER PROCEDURE [dbo].[100_InsertOptionSelection]
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @optionKey NVARCHAR(255),
    @isActive BIT = 1,
    @displayOrder INT = 0,
    @isDefaultValue BIT = 0,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Si se marca como valor por defecto, desmarcar otras opciones del mismo OptionKey
        IF @isDefaultValue = 1
        BEGIN
            UPDATE OptionSelection
            SET IsDefaultValue = 0
            WHERE OptionKey = @optionKey;
        END

        INSERT INTO OptionSelection
            (Name, NameEN, OptionKey, IsActive, DisplayOrder, IsDefaultValue, CreatedAt)
        VALUES
            (@name, @nameEN, @optionKey, @isActive, @displayOrder, @isDefaultValue, GETDATE());

        SET @id = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
        RETURN @id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO 