CREATE OR ALTER PROCEDURE [dbo].[100_UpdateMessageTemplate]
    @id INT,
    @titleES NVARCHAR(500),
    @titleEN NVARCHAR(500),
    @bodyES NVARCHAR(MAX),
    @bodyEN NVARCHAR(MAX),
    @icon NVARCHAR(255) = NULL,
    @image NVARCHAR(500) = NULL,
    @link NVARCHAR(500) = NULL,
    @useRouter BIT = 0,
    @purposeES NVARCHAR(500) = NULL,
    @purposeEN NVARCHAR(500) = NULL,
    @isActive BIT = 1,
    @updatedBy NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE [dbo].[MessageTemplate]
        SET 
            TitleES = @titleES,
            TitleEN = @titleEN,
            BodyES = @bodyES,
            BodyEN = @bodyEN,
            Icon = @icon,
            Image = @image,
            Link = @link,
            UseRouter = @useRouter,
            PurposeES = @purposeES,
            PurposeEN = @purposeEN,
            IsActive = @isActive,
            UpdatedAt = GETDATE(),
            UpdatedBy = @updatedBy
        WHERE Id = @id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

