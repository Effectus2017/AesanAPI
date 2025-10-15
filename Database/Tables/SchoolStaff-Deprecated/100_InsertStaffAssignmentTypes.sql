-- =============================================
-- Script: 100_InsertStaffAssignmentTypes
-- =============================================
-- Inserta los tipos de asignación de staff en la tabla OptionSelection
-- Estos tipos definen cómo se asigna un empleado a un sitio

-- Verificar si ya existen los tipos de asignación
IF NOT EXISTS (SELECT 1
FROM OptionSelection
WHERE OptionKey = 'staffAssignmentType')
BEGIN
    INSERT INTO OptionSelection
        (OptionKey, OptionValue, OptionValueEn, DisplayOrder, IsActive, CreatedAt)
    VALUES
        ('staffAssignmentType', 'Principal', 'Primary', 1, 1, GETDATE()),
        ('staffAssignmentType', 'Secundario', 'Secondary', 2, 1, GETDATE()),
        ('staffAssignmentType', 'Temporal', 'Temporary', 3, 1, GETDATE()),
        ('staffAssignmentType', 'Apoyo', 'Support', 4, 1, GETDATE());

    PRINT 'Tipos de asignación de staff insertados exitosamente';
END
ELSE
BEGIN
    PRINT 'Los tipos de asignación de staff ya existen en OptionSelection';
END
