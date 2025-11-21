-- =============================================
-- User-Defined Table Type: SiteOperatingDayServiceBatchType
-- Descripción: Tipo de tabla para batch insert de servicios de alimentación
-- Fecha: 2025-03-15
-- Versión: 1.0
-- =============================================

-- Eliminar el tipo si existe
IF EXISTS (SELECT * FROM sys.types WHERE name = 'SiteOperatingDayServiceBatchType' AND is_table_type = 1)
BEGIN
    DROP TYPE [dbo].[SiteOperatingDayServiceBatchType];
END
GO

-- Crear el tipo de tabla para servicios
CREATE TYPE [dbo].[SiteOperatingDayServiceBatchType] AS TABLE
(
    OperatingDayId INT NOT NULL,
    ServiceTypeId INT NOT NULL,
    ChildGroupId INT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsEnabled BIT NOT NULL,
    Comment NVARCHAR(500) NULL
);
GO

