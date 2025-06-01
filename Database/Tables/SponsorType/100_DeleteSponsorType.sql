-- 100_DeleteSponsorType.sql
-- Elimina un tipo de auspiciador por id
CREATE OR ALTER PROCEDURE [100_DeleteSponsorType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    DELETE FROM SponsorType WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END; 