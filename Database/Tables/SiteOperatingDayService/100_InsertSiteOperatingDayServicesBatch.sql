-- =============================================
-- Stored Procedure: 100_InsertSiteOperatingDayServicesBatch
-- Descripción: Inserta múltiples servicios de alimentación en batch para optimizar performance
-- Fecha: 2025-03-15
-- Versión: 1.0
-- =============================================
-- IMPORTANTE: Este stored procedure requiere que el tipo de tabla SiteOperatingDayServiceBatchType
-- exista en la base de datos. Ejecute primero el script:
-- SiteOperatingDayService/SiteOperatingDayServiceBatchType.sql
-- =============================================

-- Crear el tipo de tabla si no existe
IF NOT EXISTS (SELECT *
FROM sys.types
WHERE name = 'SiteOperatingDayServiceBatchType' AND is_table_type = 1)
BEGIN
    CREATE TYPE [dbo].[SiteOperatingDayServiceBatchType] AS TABLE
    (
        OperatingDayId INT NOT NULL,
        ServiceTypeId INT NOT NULL,
        ChildGroupId INT NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        IsEnabled BIT NOT NULL,
        Comment NVARCHAR(500) NULL
    );
END
GO

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDayServicesBatch]
    @services [dbo].[SiteOperatingDayServiceBatchType] READONLY,
    @rowsInserted INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @errorMessage NVARCHAR(4000);

    BEGIN TRY
        -- Validar que hay servicios para insertar
        IF NOT EXISTS (SELECT 1
    FROM @services)
        BEGIN
        SET @rowsInserted = 0;
        RETURN;
    END

        -- Validar que todos tienen ChildGroupId (requerido)
        IF EXISTS (SELECT 1 FROM @services WHERE ChildGroupId IS NULL OR ChildGroupId <= 0)
        BEGIN
        RAISERROR('ChildGroupId es requerido y debe ser mayor a 0 para todos los servicios', 16, 1);
        RETURN;
    END
        
        -- Validar que todos los días existen y están activos
        IF EXISTS (
            SELECT 1
    FROM @services s
        LEFT JOIN SiteOperatingDays sod ON sod.Id = s.OperatingDayId
    WHERE sod.Id IS NULL
        OR sod.IsHoliday = 1
        OR sod.IsActive = 0
        )
        BEGIN
        RAISERROR('Uno o más días de funcionamiento no existen, son feriados o inactivos', 16, 1);
        RETURN;
    END
        
        -- Validar horarios dentro del rango del día
        IF EXISTS (
            SELECT 1
    FROM @services s
        INNER JOIN SiteOperatingDays sod ON sod.Id = s.OperatingDayId
    WHERE s.StartTime < sod.StartTime
        OR s.EndTime > sod.EndTime
        OR s.StartTime >= s.EndTime
        )
        BEGIN
        RAISERROR('Uno o más servicios tienen horarios fuera del rango del día de funcionamiento', 16, 1);
        RETURN;
    END
        
        -- Validar que ServiceTypeId existe
        IF EXISTS (
            SELECT 1
    FROM @services s
        LEFT JOIN ServiceType st ON st.Id = s.ServiceTypeId AND st.IsActive = 1
    WHERE st.Id IS NULL
        )
        BEGIN
        RAISERROR('Uno o más ServiceTypeId no existen o no están activos', 16, 1);
        RETURN;
    END
        
        -- Validar duplicados (antes de insertar)
        IF EXISTS (
            SELECT 1
    FROM @services s
        INNER JOIN SiteOperatingDayService sods
        ON sods.OperatingDayId = s.OperatingDayId
            AND sods.ServiceTypeId = s.ServiceTypeId
            AND sods.ChildGroupId = s.ChildGroupId
        )
        BEGIN
        RAISERROR('Uno o más servicios ya existen para los días y grupos especificados', 16, 1);
        RETURN;
    END
        
        -- Insertar todos los servicios en batch
        INSERT INTO SiteOperatingDayService
        (
        OperatingDayId,
        ServiceTypeId,
        ChildGroupId,
        StartTime,
        EndTime,
        IsEnabled,
        Comment,
        CreatedAt
        )
    SELECT
        OperatingDayId,
        ServiceTypeId,
        ChildGroupId,
        StartTime,
        EndTime,
        IsEnabled,
        Comment,
        GETDATE()
    FROM @services;
        
        SET @rowsInserted = @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH
        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        SET @rowsInserted = 0;
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

