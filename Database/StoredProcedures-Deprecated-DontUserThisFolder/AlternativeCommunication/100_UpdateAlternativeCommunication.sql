CREATE OR ALTER PROCEDURE [dbo].[100_UpdateAlternativeCommunication]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    UPDATE AlternativeCommunication
        SET Name = @name
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END; 