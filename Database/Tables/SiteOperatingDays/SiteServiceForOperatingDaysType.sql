-- =============================================
-- User-Defined Table Type: SiteServiceForOperatingDaysType
-- Descripción: Tipo de tabla para recibir servicios que se insertarán para días de funcionamiento
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- Eliminar el tipo si existe
IF EXISTS (SELECT *
FROM sys.types
WHERE name = 'SiteServiceForOperatingDaysType' AND is_table_type = 1)
BEGIN
    DROP TYPE [dbo].[SiteServiceForOperatingDaysType];
END
GO

-- Crear el tipo de tabla para servicios
CREATE TYPE [dbo].[SiteServiceForOperatingDaysType] AS TABLE
(
    ChildGroupId INT NULL,
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

