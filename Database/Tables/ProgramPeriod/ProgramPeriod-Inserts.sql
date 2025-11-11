-- =============================================
-- Inserts iniciales para ProgramPeriod
-- Descripción: Inserta períodos de programas para años 2024, 2025, 2026
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- PAF (ID 7) y PDAM (ID 1): Julio 1 - Junio 30 del año siguiente
-- PSAV (ID 2): Últimas 2 semanas de Mayo - Primeras 2 semanas de Agosto
-- PACNA (ID 3): Año completo (1 enero - 31 diciembre) - Pendiente información específica

-- =============================================
-- PDAM (ID 1) - Períodos 2024, 2025, 2026
-- =============================================
-- PDAM 2024: 2024-07-01 a 2025-06-30
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (1, 2024, '2024-07-01', '2025-06-30', 1, GETDATE(), NULL);

-- PDAM 2025: 2025-07-01 a 2026-06-30
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (1, 2025, '2025-07-01', '2026-06-30', 1, GETDATE(), NULL);

-- PDAM 2026: 2026-07-01 a 2027-06-30
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (1, 2026, '2026-07-01', '2027-06-30', 1, GETDATE(), NULL);

-- =============================================
-- PSAV (ID 2) - Períodos 2024, 2025, 2026
-- Últimas 2 semanas de Mayo - Primeras 2 semanas de Agosto
-- =============================================
-- PSAV 2024: 
-- Último lunes de mayo 2024: 2024-05-26
-- Retroceder 1 semana: 2024-05-19 (inicio)
-- Primer viernes de agosto 2024: 2024-08-01
-- Avanzar 1 semana: 2024-08-08 (fin)
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (2, 2024, '2024-05-19', '2024-08-08', 1, GETDATE(), NULL);

-- PSAV 2025:
-- Último lunes de mayo 2025: 2025-05-26
-- Retroceder 1 semana: 2025-05-19 (inicio)
-- Primer viernes de agosto 2025: 2025-08-01
-- Avanzar 1 semana: 2025-08-08 (fin)
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (2, 2025, '2025-05-19', '2025-08-08', 1, GETDATE(), NULL);

-- PSAV 2026:
-- Último lunes de mayo 2026: 2026-05-25
-- Retroceder 1 semana: 2026-05-18 (inicio)
-- Primer viernes de agosto 2026: 2026-08-07
-- Avanzar 1 semana: 2026-08-14 (fin)
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (2, 2026, '2026-05-18', '2026-08-14', 1, GETDATE(), NULL);

-- =============================================
-- PACNA (ID 3) - Períodos 2024, 2025, 2026
-- Año completo (pendiente información específica)
-- =============================================
-- PACNA 2024: 2024-01-01 a 2024-12-31
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (3, 2024, '2024-01-01', '2024-12-31', 1, GETDATE(), NULL);

-- PACNA 2025: 2025-01-01 a 2025-12-31
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (3, 2025, '2025-01-01', '2025-12-31', 1, GETDATE(), NULL);

-- PACNA 2026: 2026-01-01 a 2026-12-31
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (3, 2026, '2026-01-01', '2026-12-31', 1, GETDATE(), NULL);

-- =============================================
-- PAF (ID 7) - Períodos 2024, 2025, 2026
-- =============================================
-- PAF 2024: 2024-07-01 a 2025-06-30
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (7, 2024, '2024-07-01', '2025-06-30', 1, GETDATE(), NULL);

-- PAF 2025: 2025-07-01 a 2026-06-30
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (7, 2025, '2025-07-01', '2026-06-30', 1, GETDATE(), NULL);

-- PAF 2026: 2026-07-01 a 2027-06-30
INSERT INTO ProgramPeriod (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
VALUES (7, 2026, '2026-07-01', '2027-06-30', 1, GETDATE(), NULL);

GO

