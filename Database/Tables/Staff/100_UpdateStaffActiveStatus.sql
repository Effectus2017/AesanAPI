-- =============================================
-- Stored Procedure: 100_UpdateStaffActiveStatus
-- =============================================
-- Actualiza el estado activo de un miembro del staff

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffActiveStatus]
    @staffId INT,
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Staff
    SET
        IsActive = @isActive,
        UpdatedAt = GETDATE()
    WHERE Id = @staffId;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END