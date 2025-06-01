-- 100_DeleteGroupType.sql
-- Elimina un tipo de grupo en la tabla GroupType
-- Última actualización: 2024-06-10

CREATE OR ALTER PROCEDURE [100_DeleteGroupType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    DELETE FROM GroupType WHERE Id = @id;
    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END; 