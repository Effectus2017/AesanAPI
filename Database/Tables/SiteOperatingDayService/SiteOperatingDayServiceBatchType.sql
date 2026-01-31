-- =============================================
-- User-Defined Table Type: SiteOperatingDayServiceBatchType
-- Descripción: Tipo de tabla para batch insert de servicios de alimentación
-- Fecha: 2025-03-15
-- Versión: 1.0
-- =============================================
-- NOTA: En SQL Server no se puede modificar un tipo; hay que borrarlo y crearlo de nuevo.
-- El tipo es referenciado por 100_InsertSiteOperatingDayServicesBatch, por eso primero
-- se elimina ese SP. Después de ejecutar este script, hay que volver a ejecutar
-- 100_InsertSiteOperatingDayServicesBatch.sql para recrear el SP.
-- =============================================

-- 1. Eliminar el SP que usa el tipo (el nombre empieza por número, se usa EXEC dinámico)
IF OBJECT_ID(N'[dbo].[100_InsertSiteOperatingDayServicesBatch]', N'P') IS NOT NULL
BEGIN
    EXEC('DROP PROCEDURE [dbo].[100_InsertSiteOperatingDayServicesBatch]');
    PRINT 'SP 100_InsertSiteOperatingDayServicesBatch eliminado (recrear después con 100_InsertSiteOperatingDayServicesBatch.sql).';
END
GO

-- 2. Eliminar el tipo si existe
IF EXISTS (SELECT * FROM sys.types WHERE name = 'SiteOperatingDayServiceBatchType' AND is_table_type = 1)
BEGIN
    DROP TYPE [dbo].[SiteOperatingDayServiceBatchType];
    PRINT 'Tipo SiteOperatingDayServiceBatchType eliminado.';
END
GO

-- Crear el tipo de tabla para servicios
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
GO

