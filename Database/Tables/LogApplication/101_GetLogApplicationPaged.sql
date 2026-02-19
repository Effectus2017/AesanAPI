-- =============================================
-- Stored Procedure: 101_GetLogApplicationPaged
-- Listado paginado para centro de logs (categoría Application).
-- Convención: parámetros y alias de salida lowercase; columnas en cuerpo CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetLogApplicationPaged]
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
        category = Category,
        timestamp = CreatedAt,
        summary = Message,
        status = Status,
        payload = Payload,
        userid = UserId,
        level = Level
    FROM [dbo].[LogApplication]
    WHERE (@from IS NULL OR CreatedAt >= @from)
      AND (@to IS NULL OR CreatedAt <= @to)
    ORDER BY CreatedAt DESC
    OFFSET @offset ROWS
    FETCH NEXT @pagesize ROWS ONLY;
END;
GO
