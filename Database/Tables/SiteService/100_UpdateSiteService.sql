-- =============================================
-- Stored Procedure: 100_UpdateSiteService
-- Descripción: Actualiza un servicio de alimentación para un sitio
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteService]
    @id INT,
    @siteId INT,
    @childGroupId INT,
    @breakfast BIT = NULL,
    @breakfastFrom TIME = NULL,
    @breakfastTo TIME = NULL,
    @lunch BIT = NULL,
    @lunchFrom TIME = NULL,
    @lunchTo TIME = NULL,
    @snackAM BIT = NULL,
    @snackAMFrom TIME = NULL,
    @snackAMTo TIME = NULL,
    @dinner BIT = NULL,
    @dinnerFrom TIME = NULL,
    @dinnerTo TIME = NULL,
    @snackPM BIT = NULL,
    @snackPMFrom TIME = NULL,
    @snackPMTo TIME = NULL,
    @snackNight BIT = NULL,
    @snackNightFrom TIME = NULL,
    @snackNightTo TIME = NULL,
    @dinnerExtended BIT = NULL,
    @dinnerExtendedFrom TIME = NULL,
    @dinnerExtendedTo TIME = NULL,
    @dinnerAtRisk BIT = NULL,
    @dinnerAtRiskFrom TIME = NULL,
    @dinnerAtRiskTo TIME = NULL,
    @snackExtended BIT = NULL,
    @snackExtendedFrom TIME = NULL,
    @snackExtendedTo TIME = NULL,
    @snackAtRisk BIT = NULL,
    @snackAtRiskFrom TIME = NULL,
    @snackAtRiskTo TIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el childGroupId pertenezca al mismo siteId (obligatorio)
    IF NOT EXISTS (SELECT 1 FROM SiteChildGroup WHERE Id = @childGroupId AND SiteId = @siteId)
    BEGIN
        RAISERROR('El grupo especificado no pertenece al sitio indicado.', 16, 1);
        RETURN;
    END;

    UPDATE SiteService
    SET 
        SiteId = @siteId,
        ChildGroupId = @childGroupId,
        Breakfast = @breakfast,
        BreakfastFrom = @breakfastFrom,
        BreakfastTo = @breakfastTo,
        Lunch = @lunch,
        LunchFrom = @lunchFrom,
        LunchTo = @lunchTo,
        SnackAM = @snackAM,
        SnackAMFrom = @snackAMFrom,
        SnackAMTo = @snackAMTo,
        Dinner = @dinner,
        DinnerFrom = @dinnerFrom,
        DinnerTo = @dinnerTo,
        SnackPM = @snackPM,
        SnackPMFrom = @snackPMFrom,
        SnackPMTo = @snackPMTo,
        SnackNight = @snackNight,
        SnackNightFrom = @snackNightFrom,
        SnackNightTo = @snackNightTo,
        DinnerExtended = @dinnerExtended,
        DinnerExtendedFrom = @dinnerExtendedFrom,
        DinnerExtendedTo = @dinnerExtendedTo,
        DinnerAtRisk = @dinnerAtRisk,
        DinnerAtRiskFrom = @dinnerAtRiskFrom,
        DinnerAtRiskTo = @dinnerAtRiskTo,
        SnackExtended = @snackExtended,
        SnackExtendedFrom = @snackExtendedFrom,
        SnackExtendedTo = @snackExtendedTo,
        SnackAtRisk = @snackAtRisk,
        SnackAtRiskFrom = @snackAtRiskFrom,
        SnackAtRiskTo = @snackAtRiskTo,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END;
