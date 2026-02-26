CREATE PROCEDURE [dbo].[100_DeleteAgencyAppointment]
    @Id INT,
    @UserId NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[AgencyAppointment]
    SET [IsDeleted] = 1, [UpdatedAt] = GETDATE(), [UpdatedBy] = @UserId
    WHERE [Id] = @Id;

END
GO
