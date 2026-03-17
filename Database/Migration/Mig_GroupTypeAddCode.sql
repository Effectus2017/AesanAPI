-- =============================================
-- Mig_GroupTypeAddCode
-- Descripción: Pobla la columna Code en GroupType para identificación estable.
-- Códigos en inglés. Ejecutar una vez tras añadir la columna Code a la tabla GroupType.
-- Fecha: 2026-03-16
-- =============================================

SET NOCOUNT ON;

UPDATE GroupType SET Code = N'NA' WHERE Name = N'N/A' AND NameEN = N'N/A';
UPDATE GroupType SET Code = N'DINING_ROOM' WHERE (Name = N'Comedor' OR NameEN = N'Dining Room');
UPDATE GroupType SET Code = N'DINING_ROOM_EXTERNAL' WHERE Name = N'Consume en Comedor-Grupo Externo' OR NameEN = N'Consume in Dining Room-External Group';
UPDATE GroupType SET Code = N'DINING_ROOM_INTERNAL' WHERE Name = N'Consume en Comedor-Grupo Interno' OR NameEN = N'Consume in Dining Room-Internal Group';
UPDATE GroupType SET Code = N'HOME' WHERE Name = N'Domicilio' AND NameEN = N'Home';
UPDATE GroupType SET Code = N'NON_CONGREGATE' WHERE (Name = N'No Congregado' OR NameEN = N'Non-Congregate');
UPDATE GroupType SET Code = N'SATELLITE' WHERE (Name = N'Satélite' OR NameEN = N'Satellite');
UPDATE GroupType SET Code = N'EXPRESS_TRAIN_CAR' WHERE (Name = N'Servi-Expreso/Carro' OR NameEN = N'Express Train/Car');
UPDATE GroupType SET Code = N'TRUCK_SERVICE' WHERE (Name = N'Servicio en Camiones' OR NameEN = N'Truck Service');
UPDATE GroupType SET Code = N'CLASSROOM' WHERE (Name = N'Salón de Clases' OR NameEN = N'Classroom');
UPDATE GroupType SET Code = N'LOUNGES' WHERE (Name = N'Salones' OR NameEN = N'Lounges');
UPDATE GroupType SET Code = N'SERVICE_CAR' WHERE (Name = N'Servi-Carro' OR NameEN = N'Service Car');
