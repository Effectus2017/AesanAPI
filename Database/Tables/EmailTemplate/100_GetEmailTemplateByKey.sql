CREATE OR ALTER PROCEDURE [dbo].[100_GetEmailTemplateByKey]
    @templateKey NVARCHAR(100)
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
        IsActive,
        CreatedAt,
        UpdatedAt,
        CreatedBy,
        UpdatedBy
    FROM [dbo].[EmailTemplate]
    WHERE TemplateKey = @templateKey
        AND IsActive = 1;
END;
GO

