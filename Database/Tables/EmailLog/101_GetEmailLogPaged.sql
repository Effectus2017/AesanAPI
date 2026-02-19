-- =============================================
-- Stored Procedure: 101_GetEmailLogPaged
-- Listado paginado para centro de logs (categoría Email).
-- Convención: parámetros y alias de salida lowercase; columnas en cuerpo CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetEmailLogPaged]
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
        category = N'Email',
        timestamp = COALESCE(AttemptedAt, CreatedAt),
        summary = Subject,
        status = Status,
        payload = CAST(RecipientEmail + N' | ' + EmailType AS NVARCHAR(MAX)),
        userid = UserId,
        level = CAST(NULL AS NVARCHAR(20))
    FROM [dbo].[EmailLog]
    WHERE (@from IS NULL OR COALESCE(AttemptedAt, CreatedAt) >= @from)
      AND (@to IS NULL OR COALESCE(AttemptedAt, CreatedAt) <= @to)
    ORDER BY COALESCE(AttemptedAt, CreatedAt) DESC
    OFFSET @offset ROWS
    FETCH NEXT @pagesize ROWS ONLY;
END;
GO
