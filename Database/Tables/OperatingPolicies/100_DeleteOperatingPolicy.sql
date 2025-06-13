-- 100_DeleteOperatingPolicy.sql
-- Elimina una política operativa por id
CREATE OR ALTER PROCEDURE [100_DeleteOperatingPolicy]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    DELETE FROM OperatingPolicy WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END;