CREATE OR ALTER PROCEDURE [dbo].[100_GetFacilityById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM dbo.Facility
    WHERE Id = @id;
END; 