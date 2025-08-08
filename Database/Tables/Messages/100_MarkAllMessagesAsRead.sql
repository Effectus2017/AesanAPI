-- =============================================
-- Stored Procedure: 100_MarkAllMessagesAsRead
-- =============================================
-- Marca todos los mensajes como leídos

CREATE OR ALTER PROCEDURE [dbo].[100_MarkAllMessagesAsRead]
    @userId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Messages
    SET [Read] = 1,
        UpdatedAt = GETDATE()
    WHERE IsDeleted = 0 AND [Read] = 0
        AND (@userId IS NULL OR UserId = @userId OR UserId IS NULL);

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END 