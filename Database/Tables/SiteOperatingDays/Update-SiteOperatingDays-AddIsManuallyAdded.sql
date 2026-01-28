-- =============================================
-- Actualización de estructura: SiteOperatingDays
-- Descripción: Agregar columna IsManuallyAdded para distinguir días generados
--              automáticamente de los agregados manualmente en el calendario
-- Fecha: 2026-01-19
-- Versión: 1.2
-- =============================================

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[SiteOperatingDays]') 
    AND name = 'IsManuallyAdded'
)
BEGIN
    ALTER TABLE [dbo].[SiteOperatingDays]
    ADD [IsManuallyAdded] [bit] NOT NULL DEFAULT 0;

    -- Agregar comentario descriptivo
    EXEC sys.sp_addextendedproperty 
        @name = N'MS_Description', 
        @value = N'Indica si el día fue agregado manualmente en el calendario (1) o generado automáticamente (0). Los días manuales no se eliminan al sincronizar con el patrón semanal.', 
        @level0type = N'SCHEMA', @level0name = N'dbo', 
        @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
        @level2type = N'COLUMN', @level2name = N'IsManuallyAdded';

    PRINT 'Columna IsManuallyAdded agregada exitosamente a SiteOperatingDays';
END
ELSE
BEGIN
    PRINT 'La columna IsManuallyAdded ya existe en SiteOperatingDays';
END
GO
