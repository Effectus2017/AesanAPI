-- =============================================
-- Stored Procedure: 101_UpdateGroupType
-- Versión 2: igual que 100_ pero incluye columna Code.
-- Parámetros en lowercase según convención.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_UpdateGroupType]
    @id INT,
    @name NVARCHAR(100),
    @nameen NVARCHAR(255),
    @isactive BIT,
    @displayorder INT,
    @code NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE GroupType
        SET Name = @name,
            NameEN = @nameen,
            IsActive = @isactive,
            DisplayOrder = @displayorder,
            Code = @code,
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
