-- =============================================
-- Script: 100_InsertStaffTypeTitles
-- =============================================
-- Inserta los tipos de staff iniciales en la tabla StaffType
-- Estos son los tipos básicos de personal que pueden existir en el sistema

INSERT INTO StaffType
    (Name, NameEn, SortOrder, IsActive, CreatedAt)
VALUES
    ('Administrativo', 'Administrative', 0, 1, GETDATE()),
    ('Operativo', 'Operational', 0, 1, GETDATE()),
    ('Directivo', 'Executive', 0, 1, GETDATE()),
    ('Técnico', 'Technical', 0, 1, GETDATE()),
    ('Supervisor', 'Supervisor', 0, 1, GETDATE()),
    ('Coordinador', 'Coordinator', 0, 1, GETDATE()),
    ('Gerente', 'Manager', 0, 1, GETDATE()),
    ('Director', 'Director', 0, 1, GETDATE()),
    ('Presidente', 'President', 0, 1, GETDATE()),
    ('Secretario', 'Secretary', 0, 1, GETDATE()),
    ('Tesorero', 'Treasurer', 0, 1, GETDATE()),
    ('Oficial', 'Officer', 0, 1, GETDATE()),
    ('Ayudante', 'Assistant', 0, 1, GETDATE());