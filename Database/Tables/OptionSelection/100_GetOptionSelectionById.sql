CREATE OR ALTER PROCEDURE [dbo].[100_GetOptionSelectionById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM OptionSelection
    WHERE Id = @id;
END;
GO 