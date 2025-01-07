CREATE OR ALTER PROCEDURE [dbo].[100_UpdateOperatingPeriod]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.OperatingPeriod
    SET Name = @name
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 