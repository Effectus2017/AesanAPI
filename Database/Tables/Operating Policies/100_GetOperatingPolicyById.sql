-- 100_GetOperatingPolicyById.sql
-- Obtiene una política operativa por id
CREATE OR ALTER PROCEDURE [100_GetOperatingPolicyById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
        Name,
        NameEN,
        IsActive,
        CreatedAt,
        UpdatedAt,
        DisplayOrder
    FROM OperatingPolicies
    WHERE Id = @id;
END;
