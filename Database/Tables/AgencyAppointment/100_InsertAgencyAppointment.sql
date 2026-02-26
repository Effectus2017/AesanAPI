CREATE PROCEDURE [dbo].[100_InsertAgencyAppointment]
    @AgencyId INT,
    @AppointmentDate DATE,
    @StartTime TIME,
    @EndTime TIME,
    @Comments NVARCHAR(1000) = NULL,
    @UserId NVARCHAR(255) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[AgencyAppointment] (
        [AgencyId],
        [AppointmentDate],
        [StartTime],
        [EndTime],
        [Comments],
        [CreatedBy],
        [UpdatedBy]
    )
    VALUES (
        @AgencyId,
        @AppointmentDate,
        @StartTime,
        @EndTime,
        @Comments,
        @UserId,
        @UserId
    );
    
    SET @id = SCOPE_IDENTITY();
END
GO
