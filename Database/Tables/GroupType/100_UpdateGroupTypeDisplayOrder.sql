-- 100_UpdateGroupTypeDisplayOrder.sql
-- Actualiza el orden de visualización de un tipo de grupo
-- Última actualización: 2024-06-10
CREATE OR ALTER PROCEDURE [100_UpdateGroupTypeDisplayOrder]
    @groupTypeId INT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    UPDATE GroupType
    SET DisplayOrder = @displayOrder,
        UpdatedAt = GETDATE()
    WHERE Id = @groupTypeId;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END; 