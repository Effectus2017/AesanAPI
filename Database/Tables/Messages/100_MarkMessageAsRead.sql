-- =============================================
-- Stored Procedure: 100_MarkMessageAsRead
-- =============================================
-- Marca un mensaje como leído

CREATE OR ALTER PROCEDURE [dbo].[100_MarkMessageAsRead]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Messages
    SET [Read] = 1,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsDeleted = 0;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END 