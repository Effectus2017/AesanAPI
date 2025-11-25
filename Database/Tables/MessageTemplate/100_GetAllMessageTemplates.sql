CREATE OR ALTER PROCEDURE [dbo].[100_GetAllMessageTemplates]
    @take INT,
    @skip INT,
    @templateKey NVARCHAR(100) = NULL,
    @purpose NVARCHAR(500) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener templates con filtros
    SELECT
        Id,
        TemplateKey,
        TitleES,
        TitleEN,
        BodyES,
        BodyEN,
        Icon,
        Image,
        Link,
        UseRouter,
        PurposeES,
        PurposeEN,
        IsActive,
        CreatedAt,
        UpdatedAt,
        CreatedBy,
        UpdatedBy
    FROM [dbo].[MessageTemplate]
    WHERE 
        (@alls = 1 OR IsActive = 1)
        AND (@templateKey IS NULL OR TemplateKey LIKE '%' + @templateKey + '%')
        AND (@purpose IS NULL OR PurposeES LIKE '%' + @purpose + '%' OR PurposeEN LIKE '%' + @purpose + '%')
    ORDER BY CreatedAt DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener total count
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[MessageTemplate]
    WHERE 
        (@alls = 1 OR IsActive = 1)
        AND (@templateKey IS NULL OR TemplateKey LIKE '%' + @templateKey + '%')
        AND (@purpose IS NULL OR PurposeES LIKE '%' + @purpose + '%' OR PurposeEN LIKE '%' + @purpose + '%');
END;
GO

