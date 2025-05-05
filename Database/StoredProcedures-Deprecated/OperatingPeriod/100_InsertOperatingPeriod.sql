CREATE OR ALTER PROCEDURE [dbo].[100_InsertOperatingPeriod]
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.OperatingPeriod (Name)
    VALUES (@name);

    SELECT SCOPE_IDENTITY() AS Id;
END; 