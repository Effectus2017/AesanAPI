-- =============================================
-- Stored Procedure: 100_GetAesanDashboardMetrics
-- Descripción: Obtiene las métricas del dashboard AESAN
-- Incluye conteos de agencias por estado y total de agencias
-- =============================================
CREATE OR ALTER PROCEDURE [100_GetAesanDashboardMetrics]
    @userId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Variables para almacenar los conteos
    DECLARE @PendingValidationCount INT = 0;
    DECLARE @OrientationCount INT = 0;
    DECLARE @ApprovedCount INT = 0;
    DECLARE @RejectedCount INT = 0;
    DECLARE @TotalAgenciesCount INT = 0;

    -- CTEs para mejorar la performance y consistencia con 117_GetAgencies
    WITH
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        )
    -- Obtener conteos por estado de agencia
    SELECT
        @PendingValidationCount = COUNT(CASE WHEN a.AgencyStatusId = 1 THEN 1 END),
        @OrientationCount = COUNT(CASE WHEN a.AgencyStatusId = 2 THEN 1 END),
        @ApprovedCount = COUNT(CASE WHEN a.AgencyStatusId = 7 THEN 1 END),
        @RejectedCount = COUNT(CASE WHEN a.AgencyStatusId = 6 THEN 1 END),
        @TotalAgenciesCount = COUNT(*)
    FROM Agency a
        LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
        LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
    WHERE a.IsActive = 1
        AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId);

    -- Retornar los resultados
    SELECT
        @PendingValidationCount AS PendingValidationCount,
        @OrientationCount AS OrientationCount,
        @ApprovedCount AS ApprovedCount,
        @RejectedCount AS RejectedCount,
        @TotalAgenciesCount AS TotalAgenciesCount,
        GETDATE() AS LastUpdated;
END

-- EXEC [100_GetAesanDashboardMetrics] @userId = '1db1104b-6c97-4f64-93e1-929296dea7bf';