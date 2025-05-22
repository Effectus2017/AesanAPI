CREATE OR ALTER PROCEDURE [dbo].[100_DeleteAlternativeCommunication]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    DELETE FROM AlternativeCommunication
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END; 