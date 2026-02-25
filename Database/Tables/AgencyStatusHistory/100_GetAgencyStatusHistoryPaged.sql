-- Historial de estados de agencia paginado.
-- Parámetros y alias de salida lowercase; columnas en cuerpo CapitalCase.
CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyStatusHistoryPaged]
    @agencyId INT,
    @from DATETIME2(7) = NULL,
    @to DATETIME2(7) = NULL,
    @page INT = 1,
    @pagesize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @offset INT = (@page - 1) * @pagesize;

    ;WITH cte AS (
        SELECT
            h.Id,
            h.AgencyId,
            h.StatusId,
            ast.Name AS StatusName,
            h.ChangedBy,
            changedbyname = RTRIM(ISNULL(s.FirstName, '') + ' ' + ISNULL(s.FatherLastName, '')),
            h.ChangedAt,
            h.Justification,
            previousstatusid = LAG(h.StatusId) OVER (PARTITION BY h.AgencyId ORDER BY h.ChangedAt),
            totalcount = COUNT(*) OVER()
        FROM AgencyStatusHistory h
        INNER JOIN AgencyStatus ast ON h.StatusId = ast.Id
        LEFT JOIN Staff s ON s.UserId = h.ChangedBy
        WHERE h.AgencyId = @agencyId
          AND (@from IS NULL OR h.ChangedAt >= @from)
          AND (@to IS NULL OR h.ChangedAt <= @to)
    ),
    cte2 AS (
        SELECT
            c.Id,
            c.AgencyId,
            c.StatusId,
            c.StatusName,
            c.ChangedBy,
            c.changedbyname,
            c.ChangedAt,
            c.Justification,
            c.previousstatusid,
            prev.Name AS previousstatusname,
            c.totalcount
        FROM cte c
        LEFT JOIN AgencyStatus prev ON prev.Id = c.previousstatusid
    )
    SELECT
        id = Id,
        agencyid = AgencyId,
        statusid = StatusId,
        statusname = StatusName,
        changedby = ChangedBy,
        changedbyname = changedbyname,
        changedat = ChangedAt,
        justification = Justification,
        previousstatusid = previousstatusid,
        previousstatusname = previousstatusname,
        totalcount = totalcount
    FROM cte2
    ORDER BY ChangedAt DESC
    OFFSET @offset ROWS
    FETCH NEXT @pagesize ROWS ONLY;
END;
GO
