CREATE OR ALTER PROCEDURE [dbo].[100_GetEmailTemplateById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        TemplateKey,
        SubjectES,
        SubjectEN,
        BodyES,
        BodyEN,
        Description,
        DescriptionEN,
        IsActive,
        CreatedAt,
        UpdatedAt,
        CreatedBy,
        UpdatedBy
    FROM [dbo].[EmailTemplate]
    WHERE Id = @id;
END;
GO

