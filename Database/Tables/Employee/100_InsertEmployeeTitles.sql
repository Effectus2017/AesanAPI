-- =============================================
-- Insertar cargos de empleados en OptionSelection
-- =============================================
-- Agregar opciones para los cargos de empleados

INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Ayudante', 'Assistant', 'employeePosition', 0, 1, 250),
    ('Presidente', 'President', 'employeePosition', 0, 1, 260),
    ('Secretario(a)', 'Secretary', 'employeePosition', 0, 1, 270),
    ('Tesorero', 'Treasurer', 'employeePosition', 0, 1, 280),
    ('Director', 'Director', 'employeePosition', 0, 1, 290),
    ('Coordinador', 'Coordinator', 'employeePosition', 0, 1, 300),
    ('Supervisor', 'Supervisor', 'employeePosition', 0, 1, 310),
    ('Gerente', 'Manager', 'employeePosition', 0, 1, 320),
    ('Administrador', 'Administrator', 'employeePosition', 0, 1, 330),
    ('Oficial', 'Officer', 'employeePosition', 0, 1, 340); 