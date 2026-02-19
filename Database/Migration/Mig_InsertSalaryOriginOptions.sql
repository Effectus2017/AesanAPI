-- =============================================
-- Migration: Insert OptionSelection for salaryOrigin (Origen del Salario)
-- =============================================
-- Opciones: Institución, Programa, Voluntario

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'salaryOrigin')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
    VALUES
        ('Institución', 'Institution', 'salaryOrigin', 0, 1, 1070),
        ('Programa', 'Program', 'salaryOrigin', 0, 1, 1080),
        ('Voluntario', 'Volunteer', 'salaryOrigin', 0, 1, 1090);
    PRINT 'Inserted salaryOrigin options (Institución, Programa, Voluntario)';
END
ELSE
BEGIN
    PRINT 'salaryOrigin options already exist';
END
