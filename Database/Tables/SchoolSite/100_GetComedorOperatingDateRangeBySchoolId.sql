-- =============================================
-- Stored Procedure: 100_GetComedorOperatingDateRangeBySchoolId
-- Descripción: Devuelve el rango de fechas de funcionamiento (OperatingFromDate, OperatingToDate)
-- del sitio tipo Comedor de la escuela. Si no hay Comedor, no devuelve filas.
-- Fecha: 2026-02-13
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetComedorOperatingDateRangeBySchoolId]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @comedorId INT = (SELECT Id FROM GroupType WHERE Code = N'DINING_ROOM');

    SELECT
        operatingfromdate = s.OperatingFromDate,
        operatingtodate = s.OperatingToDate
    FROM SchoolSite ss
        INNER JOIN Site s ON ss.SiteId = s.Id
    WHERE ss.SchoolId = @schoolId
        AND ss.IsActive = 1
        AND s.IsActive = 1
        AND s.GroupTypeId = @comedorId;
END;
