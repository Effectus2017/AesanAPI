-- =============================================
-- Script: Update SiteOperatingDayService FK
-- Descripción: Actualiza FK de ServiceTypeId para apuntar a ServiceType en lugar de OptionSelection
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

-- 1. Eliminar FK existente si existe
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SiteOperatingDayService_ServiceType')
BEGIN
    ALTER TABLE [dbo].[SiteOperatingDayService]
    DROP CONSTRAINT [FK_SiteOperatingDayService_ServiceType];
    PRINT 'FK_SiteOperatingDayService_ServiceType eliminada';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SiteOperatingDayService_OptionSelection')
BEGIN
    ALTER TABLE [dbo].[SiteOperatingDayService]
    DROP CONSTRAINT [FK_SiteOperatingDayService_OptionSelection];
    PRINT 'FK_SiteOperatingDayService_OptionSelection eliminada';
END

-- 2. Crear nueva FK apuntando a ServiceType
ALTER TABLE [dbo].[SiteOperatingDayService]
ADD CONSTRAINT [FK_SiteOperatingDayService_ServiceType]
    FOREIGN KEY ([ServiceTypeId])
    REFERENCES [dbo].[ServiceType]([Id])
    ON DELETE NO ACTION;

PRINT 'FK_SiteOperatingDayService_ServiceType creada exitosamente';
GO

