CREATE OR ALTER PROCEDURE [dbo].[100_GetAllEmailTemplates]
    @take INT,
    @skip INT,
    @templateKey NVARCHAR(100) = NULL,
    @description NVARCHAR(500) = NULL,
    @alls BIT = 0
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
    WHERE (@alls = 1 OR IsActive = 1)
        AND (@templateKey IS NULL OR TemplateKey LIKE '%' + @templateKey + '%')
        AND (@description IS NULL OR Description LIKE '%' + @description + '%')
    ORDER BY TemplateKey
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Total count
    SELECT COUNT(*)
    FROM [dbo].[EmailTemplate]
    WHERE (@alls = 1 OR IsActive = 1)
        AND (@templateKey IS NULL OR TemplateKey LIKE '%' + @templateKey + '%')
        AND (@description IS NULL OR Description LIKE '%' + @description + '%');
END;
GO

