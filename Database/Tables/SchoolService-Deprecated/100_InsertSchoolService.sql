CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchoolService]
    @schoolId INT,
    @childGroupId INT = NULL,
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
    @snackAtRiskTo TIME = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SchoolService
        (
        SchoolId, ChildGroupId,
        Breakfast, BreakfastFrom, BreakfastTo,
        Lunch, LunchFrom, LunchTo,
        SnackAM, SnackAMFrom, SnackAMTo,
        Dinner, DinnerFrom, DinnerTo,
        SnackPM, SnackPMFrom, SnackPMTo,
        SnackNight, SnackNightFrom, SnackNightTo,
        DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo,
        DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo,
        SnackExtended, SnackExtendedFrom, SnackExtendedTo,
        SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo,
        CreatedAt
        )
    VALUES
        (
            @schoolId, @childGroupId,
            @breakfast, @breakfastFrom, @breakfastTo,
            @lunch, @lunchFrom, @lunchTo,
            @snackAM, @snackAMFrom, @snackAMTo,
            @dinner, @dinnerFrom, @dinnerTo,
            @snackPM, @snackPMFrom, @snackPMTo,
            @snackNight, @snackNightFrom, @snackNightTo,
            @dinnerExtended, @dinnerExtendedFrom, @dinnerExtendedTo,
            @dinnerAtRisk, @dinnerAtRiskFrom, @dinnerAtRiskTo,
            @snackExtended, @snackExtendedFrom, @snackExtendedTo,
            @snackAtRisk, @snackAtRiskFrom, @snackAtRiskTo,
            GETDATE()
        );

    SET @id = SCOPE_IDENTITY();
END;
