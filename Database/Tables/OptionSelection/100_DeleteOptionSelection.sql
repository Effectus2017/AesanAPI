CREATE OR ALTER PROCEDURE [dbo].[100_DeleteOptionSelection]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM OptionSelection WHERE Id = @id;

    RETURN @@ROWCOUNT;
END;
GO 