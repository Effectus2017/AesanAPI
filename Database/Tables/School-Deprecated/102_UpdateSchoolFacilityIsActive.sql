-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 102_UpdateSiteFacilityIsActive
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================

CREATE PROCEDURE [dbo].[102_UpdateSchoolFacilityIsActive]
    @school_id INT,
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SchoolFacility
    SET IsActive = @is_active
    WHERE SchoolId = @school_id;
END
GO 