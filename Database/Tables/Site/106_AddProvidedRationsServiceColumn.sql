-- =============================================
-- Script de Migración: Agregar columna ProvidedRationsService a la tabla Site
-- Descripción: Agrega la columna ProvidedRationsService para almacenar si el sitio brindó servicio de raciones durante su periodo de funcionamiento
-- Fecha: 2025-01-16
-- Versión: 1.0
-- =============================================

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Site]') 
    AND name = 'ProvidedRationsService'
)
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD [ProvidedRationsService] [bit] NULL;
    
    PRINT 'Columna ProvidedRationsService agregada exitosamente a la tabla Site';
END
ELSE
BEGIN
    PRINT 'La columna ProvidedRationsService ya existe en la tabla Site';
END;
