CREATE OR ALTER PROCEDURE [dbo].[100_InsertEmailTemplate]
    @templateKey NVARCHAR(100),
    @subjectES NVARCHAR(500),
    @subjectEN NVARCHAR(500),
    @bodyES NVARCHAR(MAX),
    @bodyEN NVARCHAR(MAX),
    @description NVARCHAR(500) = NULL,
    @descriptionEN NVARCHAR(500) = NULL,
    @isActive BIT = 1,
    @createdBy NVARCHAR(450) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO [dbo].[EmailTemplate]
            (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, DescriptionEN, IsActive, CreatedAt, CreatedBy)
        VALUES
            (@templateKey, @subjectES, @subjectEN, @bodyES, @bodyEN, @description, @descriptionEN, @isActive, GETDATE(), @createdBy);

        SET @id = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        RETURN @id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

