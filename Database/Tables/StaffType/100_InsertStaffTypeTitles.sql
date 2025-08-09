-- =============================================
-- Script: 100_InsertStaffTypeTitles
-- =============================================
-- Inserta los tipos de staff principales en la tabla StaffType
-- Solo existen dos tipos de personal: Empleado y Miembro de la Junta

INSERT INTO StaffType
    (Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('Empleado', 'Employee', 1, 1, GETDATE()),
    ('Miembro de la Junta', 'Board Member', 2, 1, GETDATE());