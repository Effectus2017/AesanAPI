-- 100_UpdateGroupType.sql
-- Actualiza un tipo de grupo en la tabla GroupType
-- Convención: nombre del SP en PascalCase, parámetros y alias en lowercase
-- Última actualización: 2024-06-10

CREATE OR ALTER PROCEDURE [100_UpdateGroupType]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(255),
    @isActive BIT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE GroupType
        SET Name = @name,
            NameEN = @nameEN,
            IsActive = @isActive,
            DisplayOrder = @displayOrder,
            UpdatedAt = GETDATE()
        WHERE Id = @id;

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END; 