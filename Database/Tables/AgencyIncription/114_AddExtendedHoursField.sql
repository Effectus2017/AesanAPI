-- Script para agregar campo ExtendedHours a la tabla AgencyInscription
-- 1.1.4
-- ===========================================

-- 1. Agregar nueva columna ExtendedHours (BIT)
ALTER TABLE AgencyInscription
ADD ExtendedHours bit NULL DEFAULT 0;
GO

-- 2. Agregar comentario descriptivo
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'¿Está interesado en participar de horario extendido? (Solo para programa PACNA)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'AgencyInscription', 
    @level2type = N'COLUMN', @level2name = N'ExtendedHours';
GO

-- 3. Verificar que la nueva columna existe
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AgencyInscription'
    AND COLUMN_NAME = 'ExtendedHours';
GO

PRINT 'Campo ExtendedHours agregado exitosamente a la tabla AgencyInscription';
GO
