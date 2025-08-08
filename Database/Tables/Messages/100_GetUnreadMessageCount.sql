-- =============================================
-- Stored Procedure: 100_GetUnreadMessageCount
-- =============================================
-- Obtiene el conteo de mensajes no leídos

CREATE OR ALTER PROCEDURE [dbo].[100_GetUnreadMessageCount]
    @userId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM Messages
    WHERE IsDeleted = 0 AND [Read] = 0
        AND (@userId IS NULL OR UserId = @userId OR UserId IS NULL);
END 