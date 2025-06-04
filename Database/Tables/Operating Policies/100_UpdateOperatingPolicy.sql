-- 100_UpdateOperatingPolicy.sql
-- Actualiza una política operativa
CREATE OR ALTER PROCEDURE [100_UpdateOperatingPolicy]
    @id INT,
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    UPDATE OperatingPolicies
    SET Name = @name,
        NameEN = @nameEN,
        IsActive = @isActive,
        DisplayOrder = @displayOrder,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END;