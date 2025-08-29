-- =============================================
-- Migration Script: Add Comment Field to StaffRelationship
-- =============================================
-- Agrega el campo Comment a la tabla StaffRelationship
-- Este campo permite agregar comentarios sobre cambios de estado o la relación

-- Agregar el campo Comment
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'StaffRelationship' AND COLUMN_NAME = 'Comment')
BEGIN
    ALTER TABLE StaffRelationship
    ADD Comment NVARCHAR(500) NULL;

    PRINT 'Campo Comment agregado a la tabla StaffRelationship';
END
ELSE
BEGIN
    PRINT 'El campo Comment ya existe en la tabla StaffRelationship';
END

-- Verificar que el campo se agregó correctamente
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'StaffRelationship'
    AND COLUMN_NAME = 'Comment';
