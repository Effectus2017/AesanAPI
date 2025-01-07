CREATE OR ALTER PROCEDURE [dbo].[100_DeleteOperatingPeriod]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.OperatingPeriod
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 