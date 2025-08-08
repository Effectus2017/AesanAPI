-- =============================================
-- Stored Procedure: 100_GetAllMessages
-- =============================================
-- Obtiene todos los mensajes del usuario

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllMessages]
    @userId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Icon,
        Image,
        Title,
        Description,
        Time,
        Link,
        UseRouter,
        [Read],
        UserId,
        CreatedAt,
        UpdatedAt,
        IsDeleted
    FROM Messages
    WHERE IsDeleted = 0
        AND (@userId IS NULL OR UserId = @userId OR UserId IS NULL)
    ORDER BY Time DESC;
END 