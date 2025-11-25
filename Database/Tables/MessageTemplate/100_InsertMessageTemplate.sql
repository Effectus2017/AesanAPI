CREATE OR ALTER PROCEDURE [dbo].[100_InsertMessageTemplate]
    @templateKey NVARCHAR(100),
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
    @createdBy NVARCHAR(450) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO [dbo].[MessageTemplate]
        (TemplateKey, TitleES, TitleEN, BodyES, BodyEN, Icon, Image, Link, UseRouter, PurposeES, PurposeEN, IsActive, CreatedAt, CreatedBy)
    VALUES
        (@templateKey, @titleES, @titleEN, @bodyES, @bodyEN, @icon, @image, @link, @useRouter, @purposeES, @purposeEN, @isActive, GETDATE(), @createdBy);

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

