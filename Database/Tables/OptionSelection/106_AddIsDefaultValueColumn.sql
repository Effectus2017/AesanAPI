/*
===========================================
Script de Migración: Agregar columna IsDefaultValue
===========================================
Agrega la columna IsDefaultValue a la tabla OptionSelection para permitir
marcar qué opción debe ser seleccionada por defecto en los formularios.

Versión: 1.0
Fecha: 2025-01-XX

Descripción:
- Agrega la columna IsDefaultValue BIT NOT NULL DEFAULT 0
- Solo puede haber una opción por defecto por OptionKey
*/

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('OptionSelection')
    AND name = 'IsDefaultValue'
)
BEGIN
    ALTER TABLE OptionSelection
    ADD IsDefaultValue BIT NOT NULL DEFAULT 0;
    
    PRINT 'Columna IsDefaultValue agregada exitosamente a la tabla OptionSelection';
END
ELSE
BEGIN
    PRINT 'La columna IsDefaultValue ya existe en la tabla OptionSelection';
END
GO
