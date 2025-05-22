CREATE OR ALTER PROCEDURE [dbo].[100_GetOperatingPolicyById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Description
    FROM dbo.OperatingPolicy
    WHERE Id = @id;
END; 