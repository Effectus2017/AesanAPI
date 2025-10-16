-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 102_UpdateSatelliteSiteIsActive
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================

CREATE PROCEDURE [dbo].[102_UpdateSatelliteSchoolIsActive]
    @ma INT,
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SatelliteSchool
    SET IsActive = @is_active
    WHERE MainSchoolId = @main_school_id;
END
GO 