CREATE PROCEDURE [dbo].[100_UpdateAgencyAppointment]
    @Id INT,
    @AppointmentDate DATE,
    @StartTime TIME,
    @EndTime TIME,
    @Comments NVARCHAR(1000) = NULL,
    @UserId NVARCHAR(255) = NULL,
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[AgencyAppointment]
    SET 
        [AppointmentDate] = @AppointmentDate,
        [StartTime] = @StartTime,
        [EndTime] = @EndTime,
        [Comments] = @Comments,
        [UpdatedAt] = GETDATE(),
        [UpdatedBy] = @UserId
    WHERE [Id] = @Id AND [IsDeleted] = 0;
    
    SET @rowsAffected = @@ROWCOUNT;
END
GO
