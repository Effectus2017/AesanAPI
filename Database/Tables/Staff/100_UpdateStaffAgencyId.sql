-- =============================================
-- Stored Procedure: 100_UpdateStaffAgencyId
-- =============================================
-- Actualiza solo el AgencyId de un miembro del staff

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffAgencyId]
    @staffId INT,
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Staff
    SET AgencyId = @agencyId,
        UpdatedAt = GETDATE()
    WHERE Id = @staffId;

    RETURN @@ROWCOUNT;
END
