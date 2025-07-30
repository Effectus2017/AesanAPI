-- =============================================
-- Stored Procedure: 100_ConvertStaffToUser
-- =============================================
-- Convierte un miembro del staff en usuario del sistema
-- Asocia el UserId al staff para indicar que ya tiene un usuario

CREATE OR ALTER PROCEDURE [dbo].[100_ConvertStaffToUser]
    @staffId INT,
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Staff
    SET
        UserId = @userId,
        UpdatedAt = GETDATE()
    WHERE Id = @staffId;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END