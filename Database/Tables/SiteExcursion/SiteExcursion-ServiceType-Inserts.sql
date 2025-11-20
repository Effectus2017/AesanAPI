-- =============================================
-- Script: Insertar Tipos de Servicios para Excursiones
-- Descripción: Inserta los tipos de servicios en OptionSelection para excursiones
-- OptionKey: 'service-type'
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

-- Verificar e insertar tipos de servicios si no existen
IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'service-type' AND Name = 'Desayuno')
BEGIN
    INSERT INTO OptionSelection (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
    VALUES ('Desayuno', 'Breakfast', 'service-type', 0, 1, 200);
END

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'service-type' AND Name = 'Almuerzo')
BEGIN
    INSERT INTO OptionSelection (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
    VALUES ('Almuerzo', 'Lunch', 'service-type', 0, 1, 300);
END

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'service-type' AND Name = 'Merienda Matutina')
BEGIN
    INSERT INTO OptionSelection (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
    VALUES ('Merienda Matutina', 'Morning Snack', 'service-type', 0, 1, 400);
END

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'service-type' AND Name = 'Cena')
BEGIN
    INSERT INTO OptionSelection (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
    VALUES ('Cena', 'Dinner', 'service-type', 0, 1, 500);
END

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'service-type' AND Name = 'Merienda Vespertina')
BEGIN
    INSERT INTO OptionSelection (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
    VALUES ('Merienda Vespertina', 'Afternoon Snack', 'service-type', 0, 1, 600);
END

