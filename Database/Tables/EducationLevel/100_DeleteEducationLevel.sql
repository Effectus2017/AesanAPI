CREATE OR ALTER PROCEDURE [dbo].[100_DeleteEducationLevel]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    DELETE FROM EducationLevel
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO