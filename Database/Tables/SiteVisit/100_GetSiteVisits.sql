-- =============================================
-- SP: 100_GetSiteVisits — visitas por agencia/sitio y mes/año
-- Resultset 1: agencia | Resultset 2: visitas
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteVisits]
    @agencyid INT,
    @siteid INT,
    @month INT = NULL,
    @year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        agencyId = a.Id,
        agencyName = a.Name
    FROM [dbo].[Agency] a
    WHERE a.Id = @agencyid;

    SELECT
        id = sv.Id,
        siteId = sv.SiteId,
        siteName = s.Name,
        visitTypeId = sv.VisitTypeId,
        visitTypeCode = vt.Code,
        visitTypeNameEs = vt.NameEs,
        visitTypeNameEN = vt.NameEN,
        date = CONVERT(NVARCHAR(10), sv.VisitDate, 126),
        startTime = CONVERT(NVARCHAR(8), sv.StartTime, 108),
        endTime = CONVERT(NVARCHAR(8), sv.EndTime, 108),
        comment = sv.Comments
    FROM [dbo].[SiteVisit] sv
    INNER JOIN [dbo].[Site] s ON s.Id = sv.SiteId
    INNER JOIN [dbo].[VisitType] vt ON vt.Id = sv.VisitTypeId
    WHERE sv.IsDeleted = 0
      AND s.AgencyId = @agencyid
      AND s.IsActive = 1
      AND sv.SiteId = @siteid
      AND (@month IS NULL OR MONTH(sv.VisitDate) = @month)
      AND (@year IS NULL OR YEAR(sv.VisitDate) = @year)
    ORDER BY sv.VisitDate ASC, sv.StartTime ASC;
END
GO
