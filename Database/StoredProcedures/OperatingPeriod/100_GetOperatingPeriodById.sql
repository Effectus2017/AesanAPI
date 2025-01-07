CREATE OR ALTER PROCEDURE [dbo].[100_GetOperatingPeriodById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM dbo.OperatingPeriod
    WHERE Id = @id;
END; 