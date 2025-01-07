CREATE OR ALTER PROCEDURE [dbo].[100_UpdateFacility]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Facility
    SET Name = @name
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 