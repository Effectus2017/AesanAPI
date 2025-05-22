CREATE OR ALTER PROCEDURE [dbo].[100_InsertOperatingPolicy]
    @description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.OperatingPolicy (Description)
    VALUES (@description);

    SELECT SCOPE_IDENTITY() AS Id;
END; 