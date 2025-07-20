-- =============================================
-- Insertar títulos de empleados en OptionSelection
-- =============================================
-- Agregar opciones para los títulos de empleados

INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Ayudante', 'Assistant', 'employeeTitle', 0, 1, 10),
    ('Presidente', 'President', 'employeeTitle', 0, 1, 20),
    ('Secretario(a)', 'Secretary', 'employeeTitle', 0, 1, 30),
    ('Tesorero', 'Treasurer', 'employeeTitle', 0, 1, 40),
    ('Director', 'Director', 'employeeTitle', 0, 1, 50),
    ('Coordinador', 'Coordinator', 'employeeTitle', 0, 1, 60),
    ('Supervisor', 'Supervisor', 'employeeTitle', 0, 1, 70),
    ('Gerente', 'Manager', 'employeeTitle', 0, 1, 80),
    ('Administrador', 'Administrator', 'employeeTitle', 0, 1, 90),
    ('Oficial', 'Officer', 'employeeTitle', 0, 1, 100); 