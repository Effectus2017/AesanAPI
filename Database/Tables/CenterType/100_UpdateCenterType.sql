-- 100_UpdateCenterType
-- Actualiza un tipo de centro
CREATE OR ALTER PROCEDURE [100_UpdateCenterType]
    @id INT,
    @name NVARCHAR(255),
    @nameEN NVARCHAR(255),
    @displayOrder INT,
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE CenterType
    SET Name = @name,
        NameEN = @nameEN,
        DisplayOrder = @displayOrder,
        IsActive = @isActive,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END;
GO 