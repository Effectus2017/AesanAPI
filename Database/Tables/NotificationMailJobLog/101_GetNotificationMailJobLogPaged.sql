-- =============================================
-- Stored Procedure: 101_GetNotificationMailJobLogPaged
-- Listado paginado para centro de logs (categoría Job).
-- Convención: parámetros y alias de salida lowercase; columnas en cuerpo CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetNotificationMailJobLogPaged]
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
        id = Id,
        category = N'Job',
        timestamp = StartedAt,
        summary = JobName + N' - ' + COALESCE(Message, N''),
        status = Status,
        payload = Message,
        userid = CAST(NULL AS NVARCHAR(450)),
        level = CAST(NULL AS NVARCHAR(20))
    FROM [dbo].[NotificationMailJobLog]
    WHERE (@from IS NULL OR StartedAt >= @from)
      AND (@to IS NULL OR StartedAt <= @to)
    ORDER BY StartedAt DESC
    OFFSET @offset ROWS
    FETCH NEXT @pagesize ROWS ONLY;
END;
GO
