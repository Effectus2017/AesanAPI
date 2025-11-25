CREATE OR ALTER PROCEDURE [dbo].[100_GetMessageTemplateById]
    @id INT
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
    WHERE Id = @id;
END;
GO

