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

    UPDATE OptionSelection
    SET Name = @name,
        NameEN = @nameEN,
        OptionKey = @optionKey,
        IsActive = @isActive,
        DisplayOrder = @displayOrder,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    RETURN @@ROWCOUNT;
END;
GO 