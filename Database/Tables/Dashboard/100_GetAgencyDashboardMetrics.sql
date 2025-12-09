-- =============================================
-- Stored Procedure: 100_GetAgencyDashboardMetrics
-- Descripción: Obtiene las métricas del dashboard de agencia
-- Incluye conteo de sitios y escuelas activas de la agencia
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyDashboardMetrics]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Variables para almacenar los conteos
    DECLARE @TotalSites INT = 0;
    DECLARE @TotalSchools INT = 0;

    -- Contar sitios activos de la agencia
    SELECT @TotalSites = COUNT(*)
    FROM [Site] s
    WHERE s.[AgencyId] = @agencyId
        AND s.[IsActive] = 1;

    -- Contar escuelas activas de la agencia
    SELECT @TotalSchools = COUNT(*)
    FROM [School] s
    WHERE s.[AgencyId] = @agencyId
        AND s.[IsActive] = 1;

    -- Retornar los resultados
    SELECT
        @TotalSites AS TotalSites,
        @TotalSchools AS TotalSchools,
        GETUTCDATE() AS LastUpdated;
END;

-- Ejemplo de ejecución:
-- EXEC [dbo].[100_GetAgencyDashboardMetrics] @agencyId = 1;

