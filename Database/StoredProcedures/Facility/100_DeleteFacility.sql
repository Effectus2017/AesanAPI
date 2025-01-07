CREATE OR ALTER PROCEDURE [dbo].[100_DeleteFacility]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Facility
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END; 