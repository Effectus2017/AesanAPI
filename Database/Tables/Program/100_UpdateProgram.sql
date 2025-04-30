-- Procedimiento para actualizar un programa existente
CREATE OR ALTER PROCEDURE [100_UpdateProgram]
    @id INT,
    @name NVARCHAR(255),
    @description NVARCHAR(MAX),
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @rowsAffected INT;

    UPDATE Program
    SET Name = @name,
        Description = @description,
        IsActive = @isActive,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 