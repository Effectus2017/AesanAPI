CREATE OR ALTER PROCEDURE [dbo].[105_UpdateOptionSelectionDisplayOrder]
    @optionSelectionId INT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE OptionSelection
    SET DisplayOrder = @displayOrder,
        UpdatedAt = GETDATE()
    WHERE Id = @optionSelectionId;

    RETURN @@ROWCOUNT;
END;
GO 