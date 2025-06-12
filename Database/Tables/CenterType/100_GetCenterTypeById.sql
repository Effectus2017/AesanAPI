-- 100_GetCenterTypeById
-- Obtiene un tipo de centro por su ID
CREATE OR ALTER PROCEDURE [100_GetCenterTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, DisplayOrder, IsActive, CreatedAt, UpdatedAt
    FROM CenterType
    WHERE Id = @id;
END;
GO 