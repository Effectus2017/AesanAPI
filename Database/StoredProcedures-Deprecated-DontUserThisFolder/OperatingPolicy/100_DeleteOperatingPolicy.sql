CREATE OR ALTER PROCEDURE [dbo].[100_DeleteOperatingPolicy]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.OperatingPolicy
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 