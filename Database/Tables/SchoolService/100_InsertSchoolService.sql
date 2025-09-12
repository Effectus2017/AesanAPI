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
            GETDATE()
        );

    SET @id = SCOPE_IDENTITY();
END;
