-- =============================================
-- Migration: Add OperatingStartTime and OperatingEndTime to Site table
-- Descripción: Agrega las columnas OperatingStartTime y OperatingEndTime a la tabla Site
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

-- Verificar si las columnas ya existen antes de agregarlas
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Site]') AND name = 'OperatingStartTime')
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD [OperatingStartTime] [time] NULL;
    
    PRINT 'Columna OperatingStartTime agregada exitosamente a la tabla Site';
END
ELSE
BEGIN
    PRINT 'La columna OperatingStartTime ya existe en la tabla Site';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Site]') AND name = 'OperatingEndTime')
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD [OperatingEndTime] [time] NULL;
    
    PRINT 'Columna OperatingEndTime agregada exitosamente a la tabla Site';
END
ELSE
BEGIN
    PRINT 'La columna OperatingEndTime ya existe en la tabla Site';
END
GO

