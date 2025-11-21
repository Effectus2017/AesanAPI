CREATE OR ALTER PROCEDURE [dbo].[100_UpdateEmailTemplate]
    @id INT,
    @subjectES NVARCHAR(500),
    @subjectEN NVARCHAR(500),
    @bodyES NVARCHAR(MAX),
    @bodyEN NVARCHAR(MAX),
    @description NVARCHAR(500) = NULL,
    @isActive BIT,
    @updatedBy NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;

    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE [dbo].[EmailTemplate]
        SET SubjectES = @subjectES,
            SubjectEN = @subjectEN,
            BodyES = @bodyES,
            BodyEN = @bodyEN,
            Description = @description,
            IsActive = @isActive,
            UpdatedAt = GETDATE(),
            UpdatedBy = @updatedBy
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

