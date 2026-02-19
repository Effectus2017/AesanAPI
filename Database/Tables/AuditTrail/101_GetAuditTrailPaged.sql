-- =============================================
-- Stored Procedure: 101_GetAuditTrailPaged
-- Listado paginado para centro de logs (categoría Audit).
-- Convención: parámetros y alias de salida lowercase; columnas en cuerpo CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetAuditTrailPaged]
    @from DATETIME2(7) = NULL,
    @to DATETIME2(7) = NULL,
    @page INT = 1,
    @pagesize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @offset INT = (@page - 1) * @pagesize;

    SELECT
        totalcount = COUNT(*) OVER(),
        id = at.Id,
        category = N'Audit',
        timestamp = at.ChangedAt,
        summary = at.Action + N' ' + at.TableName + N' ' + at.EntityId,
        status = aos.Status,
        payload = at.NewValues,
        userid = at.ChangedBy,
        level = CAST(NULL AS NVARCHAR(20))
    FROM [dbo].[AuditTrail] at
    LEFT JOIN [dbo].[AuditOperationSummary] aos ON at.OperationId = aos.OperationId
    WHERE (@from IS NULL OR at.ChangedAt >= @from)
      AND (@to IS NULL OR at.ChangedAt <= @to)
    ORDER BY at.ChangedAt DESC
    OFFSET @offset ROWS
    FETCH NEXT @pagesize ROWS ONLY;
END;
GO
