CREATE PROCEDURE [dbo].[100_GetAgencyAppointments]
    @AgencyId INT,
    @Month INT = NULL,
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Return agency info
    SELECT 
        [Id] AS AgencyId,
        [Name] AS AgencyName
    FROM [dbo].[Agency]
    WHERE [Id] = @AgencyId;

    -- Return appointments
    SELECT 
        A.[Id],
        A.[AgencyId],
        A.[AppointmentDate] AS [Date],
        A.[StartTime],
        A.[EndTime],
        A.[Comments] AS [Comment]
    FROM [dbo].[AgencyAppointment] A
    WHERE A.[AgencyId] = @AgencyId
      AND A.[IsDeleted] = 0
      AND (@Month IS NULL OR MONTH(A.[AppointmentDate]) = @Month)
      AND (@Year IS NULL OR YEAR(A.[AppointmentDate]) = @Year)
    ORDER BY A.[AppointmentDate] ASC, A.[StartTime] ASC;

END
GO
