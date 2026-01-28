-- =============================================
-- Script: 107_AddDiningRoomCapacityColumn
-- Descripción: Agrega la columna DiningRoomCapacity a la tabla Site
-- Fecha: 2025-01-16
-- Versión: 1.0
-- =============================================

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Site') 
    AND name = 'DiningRoomCapacity'
)
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD [DiningRoomCapacity] [int] NULL;
    
    PRINT 'Columna DiningRoomCapacity agregada exitosamente a la tabla Site';
END
ELSE
BEGIN
    PRINT 'La columna DiningRoomCapacity ya existe en la tabla Site';
END;
