-- =============================================
-- Actualización de estructura: SiteOperatingDays
-- Descripción: Agregar campos faltantes para compatibilidad con SPs migrados
-- Fecha: 2025-01-15
-- Versión: 1.1
-- =============================================

-- Agregar campos faltantes
ALTER TABLE [dbo].[SiteOperatingDays]
ADD [IsWeekendOverride] [bit] NOT NULL DEFAULT 0;

ALTER TABLE [dbo].[SiteOperatingDays]
ADD [IsExcluded] [bit] NOT NULL DEFAULT 0;

ALTER TABLE [dbo].[SiteOperatingDays]
ADD [IsHoliday] [bit] NOT NULL DEFAULT 0;

-- Cambiar tamaño de Comment para compatibilidad
ALTER TABLE [dbo].[SiteOperatingDays]
ALTER COLUMN [Comment] [nvarchar](500) NULL;

-- Agregar restricción única para evitar duplicados
ALTER TABLE [dbo].[SiteOperatingDays]
ADD CONSTRAINT [UQ_SiteOperatingDays_Site_Date] 
UNIQUE ([SiteId], [OperatingDate]);

-- =============================================
-- Comentarios para los nuevos campos
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'true si es fin de semana pero funciona por excepción', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'IsWeekendOverride';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Excluir día (no operativo)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'IsExcluded';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'true si es día feriado', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'IsHoliday';
