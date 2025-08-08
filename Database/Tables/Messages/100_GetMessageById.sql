-- =============================================
-- Stored Procedure: 100_GetMessageById
-- =============================================
-- Obtiene un mensaje específico por su ID

CREATE OR ALTER PROCEDURE [dbo].[100_GetMessageById]
    @id INT
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
    WHERE Id = @id AND IsDeleted = 0;
END 