/*
===========================================
Script de Migración: Establecer valores por defecto existentes
===========================================
Marca los valores por defecto identificados en el código para opciones
que actualmente se establecen como por defecto en formularios.

Versión: 1.0
Fecha: 2025-01-XX

Descripción:
- Marca N/A como valor por defecto para headStartProgram
- Este script debe ejecutarse después de agregar la columna IsDefaultValue
*/

-- Marcar N/A como valor por defecto para headStartProgram
-- (usado en sign-up.component.ts cuando el programa es PSAV)
UPDATE OptionSelection
SET IsDefaultValue = 1
WHERE OptionKey = 'headStartProgram' 
  AND (Name = 'N/A' OR NameEN = 'N/A');

PRINT 'Valores por defecto establecidos exitosamente';
GO
