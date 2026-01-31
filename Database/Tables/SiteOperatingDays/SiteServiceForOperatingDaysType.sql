-- =============================================
-- User-Defined Table Type: SiteServiceForOperatingDaysType
-- Descripción: Tipo de tabla para recibir servicios que se insertarán para días de funcionamiento
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================
-- NOTA: En SQL Server no se puede modificar un tipo; hay que borrarlo y crearlo de nuevo.
-- El tipo es referenciado por 100_InsertServicesForOperatingDays, por eso primero se
-- elimina ese SP. Después de ejecutar este script, hay que volver a ejecutar
-- 100_InsertServicesForOperatingDays.sql para recrear el SP.
-- =============================================

-- 1. Eliminar el SP que usa el tipo (el nombre empieza por número, se usa EXEC dinámico)
IF OBJECT_ID(N'[dbo].[100_InsertServicesForOperatingDays]', N'P') IS NOT NULL
BEGIN
    EXEC('DROP PROCEDURE [dbo].[100_InsertServicesForOperatingDays]');
    PRINT 'SP 100_InsertServicesForOperatingDays eliminado (recrear después con 100_InsertServicesForOperatingDays.sql).';
END
GO

-- 2. Eliminar el tipo si existe
IF EXISTS (SELECT *
FROM sys.types
WHERE name = 'SiteServiceForOperatingDaysType' AND is_table_type = 1)
BEGIN
    DROP TYPE [dbo].[SiteServiceForOperatingDaysType];
    PRINT 'Tipo SiteServiceForOperatingDaysType eliminado.';
END
GO

-- Crear el tipo de tabla para servicios
CREATE TYPE [dbo].[SiteServiceForOperatingDaysType] AS TABLE
(
    ChildGroupId INT NOT NULL,
    -- Breakfast
    Breakfast BIT NULL,
    BreakfastFrom TIME NULL,
    BreakfastTo TIME NULL,
    -- Lunch
    Lunch BIT NULL,
    LunchFrom TIME NULL,
    LunchTo TIME NULL,
    -- SnackAM
    SnackAM BIT NULL,
    SnackAMFrom TIME NULL,
    SnackAMTo TIME NULL,
    -- Dinner
    Dinner BIT NULL,
    DinnerFrom TIME NULL,
    DinnerTo TIME NULL,
    -- SnackPM
    SnackPM BIT NULL,
    SnackPMFrom TIME NULL,
    SnackPMTo TIME NULL,
    -- SnackNight
    SnackNight BIT NULL,
    SnackNightFrom TIME NULL,
    SnackNightTo TIME NULL,
    -- DinnerExtended (PACNA)
    DinnerExtended BIT NULL,
    DinnerExtendedFrom TIME NULL,
    DinnerExtendedTo TIME NULL,
    -- DinnerAtRisk (PACNA)
    DinnerAtRisk BIT NULL,
    DinnerAtRiskFrom TIME NULL,
    DinnerAtRiskTo TIME NULL,
    -- SnackExtended (PACNA)
    SnackExtended BIT NULL,
    SnackExtendedFrom TIME NULL,
    SnackExtendedTo TIME NULL,
    -- SnackAtRisk (PACNA)
    SnackAtRisk BIT NULL,
    SnackAtRiskFrom TIME NULL,
    SnackAtRiskTo TIME NULL
);
GO

