CREATE OR ALTER PROCEDURE [dbo].[100_UpdateOperatingPolicy]
    @id INT,
    @description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.OperatingPolicy
    SET Description = @description
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 