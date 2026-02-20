-- =============================================
-- Migration: Add "Ambos" (Both) to StaffClassification
-- =============================================
-- Inserta la clasificación "Ambos" para empleados con doble clasificación.

IF NOT EXISTS (SELECT 1 FROM StaffClassification WHERE Name = N'Ambos')
BEGIN
    INSERT INTO StaffClassification
        (Name, NameEn, SortOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (N'Ambos', N'Both', 3, 1, GETDATE(), NULL);
    PRINT 'Inserted StaffClassification: Ambos (Both)';
END
ELSE
BEGIN
    PRINT 'StaffClassification Ambos already exists';
END
