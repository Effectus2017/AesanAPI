-- =============================================
-- Stored Procedure: 100_GetAllAgencyStatusHistory
-- =============================================
-- Obtiene todo el historial de estados de agencia con paginación y filtros.
-- Parámetros:
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @agencyid: ID de la agencia (opcional)
--   @from: Fecha desde (opcional)
--   @to: Fecha hasta (opcional)

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllAgencyStatusHistory]
    @take INT = 20,
    @skip INT = 0,
    @agencyid INT = NULL,
    @from DATETIME2(7) = NULL,
    @to DATETIME2(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH cte AS (
        SELECT
            h.Id,
            h.AgencyId,
            a.Name AS AgencyName,
            h.StatusId,
            ast.Name AS StatusName,
            h.ChangedBy,
            RTRIM(ISNULL(s.FirstName, '') + ' ' + ISNULL(s.FatherLastName, '')) AS ChangedByName,
            h.ChangedAt,
            h.Justification
        FROM AgencyStatusHistory h
        INNER JOIN Agency a ON h.AgencyId = a.Id
        INNER JOIN AgencyStatus ast ON h.StatusId = ast.Id
        LEFT JOIN Staff s ON s.UserId = h.ChangedBy
        WHERE (@agencyid IS NULL OR h.AgencyId = @agencyid)
          AND (@from IS NULL OR h.ChangedAt >= @from)
          AND (@to IS NULL OR h.ChangedAt <= @to)
    )
    SELECT
        Id,
        AgencyId,
        AgencyName,
        StatusId,
        StatusName,
        ChangedBy,
        ChangedByName,
        ChangedAt,
        Justification
    FROM cte
    ORDER BY ChangedAt DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Segundo result set: total de registros (misma fórmula que 100_GetAllStaff / 109_GetAllUsersFromDb).
    SELECT COUNT(*)
    FROM AgencyStatusHistory h
    WHERE (@agencyid IS NULL OR h.AgencyId = @agencyid)
      AND (@from IS NULL OR h.ChangedAt >= @from)
      AND (@to IS NULL OR h.ChangedAt <= @to);
END;
GO
