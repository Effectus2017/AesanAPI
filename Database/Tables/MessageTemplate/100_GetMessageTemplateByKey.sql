CREATE OR ALTER PROCEDURE [dbo].[100_GetMessageTemplateByKey]
    @templateKey NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

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
    WHERE TemplateKey = @templateKey
        AND IsActive = 1;
END;
GO

