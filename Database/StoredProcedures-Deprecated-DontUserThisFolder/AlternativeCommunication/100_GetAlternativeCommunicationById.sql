CREATE OR ALTER PROCEDURE [dbo].[100_GetAlternativeCommunicationById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM AlternativeCommunication
    WHERE Id = @id;
END; 