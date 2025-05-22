CREATE OR ALTER PROCEDURE [dbo].[100_InsertFacility]
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Facility (Name)
    VALUES (@name);

    SELECT SCOPE_IDENTITY() AS Id;
END; 