-- =============================================
-- Stored Procedure: 101_UpdateSiteOperatingDayServiceTimesByGroupAndServiceType
-- Descripción: Actualiza StartTime y EndTime de todos los SiteOperatingDayService
--              de un grupo y tipo de servicio dados.
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteOperatingDayServiceTimesByGroupAndServiceType]
    @childgroupid INT,
    @servicetypeid INT,
    @starttime TIME,
    @endtime TIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SiteOperatingDayService
    SET
        StartTime = @starttime,
        EndTime = @endtime,
        UpdatedAt = GETDATE()
    WHERE ChildGroupId = @childgroupid
      AND ServiceTypeId = @servicetypeid;
END;
