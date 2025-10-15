CREATE OR ALTER PROCEDURE [dbo].[100_UpdateEducationLevel]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE EducationLevel
        SET Name = @name,
            NameEN = @nameEN,
            IsActive = @isActive,
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
GO 