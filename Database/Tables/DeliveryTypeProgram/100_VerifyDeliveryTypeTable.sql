-- Script de verificación de la estructura de la tabla DeliveryType
-- Verifica si la columna SelectionNotification existe y la elimina si es necesario

-- Paso 1: Verificar si la columna SelectionNotification existe
IF EXISTS (
    SELECT COLUMN_NAME 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'DeliveryType' AND COLUMN_NAME = 'SelectionNotification'
)
BEGIN
    PRINT 'ADVERTENCIA: La columna SelectionNotification existe en la tabla DeliveryType.';
    PRINT 'Eliminando la columna...';
    
    -- Primero eliminar el constraint por defecto si existe
    DECLARE @ConstraintName NVARCHAR(200);
    SELECT @ConstraintName = name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('DeliveryType')
        AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('DeliveryType'), 'SelectionNotification', 'ColumnId');
    
    IF @ConstraintName IS NOT NULL
    BEGIN
        DECLARE @DropConstraintSQL NVARCHAR(MAX);
        SET @DropConstraintSQL = 'ALTER TABLE DeliveryType DROP CONSTRAINT ' + @ConstraintName;
        EXEC sp_executesql @DropConstraintSQL;
        PRINT 'Constraint eliminado: ' + @ConstraintName;
    END
    
    -- Eliminar la columna
    ALTER TABLE DeliveryType DROP COLUMN SelectionNotification;
    PRINT 'Columna SelectionNotification eliminada correctamente.';
END
ELSE
BEGIN
    PRINT 'La columna SelectionNotification no existe en la tabla DeliveryType.';
    PRINT 'No se requiere ninguna acción.';
END
GO

-- Paso 2: Verificar la estructura actual de la tabla
PRINT 'Estructura actual de la tabla DeliveryType:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DeliveryType'
ORDER BY ORDINAL_POSITION;
GO

