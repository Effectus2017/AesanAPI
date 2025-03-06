CREATE OR ALTER PROCEDURE [dbo].[100_UnassignAgencyFromUser]
    @userId NVARCHAR(450),
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE UserAgencyAssignment
    SET IsActive = 0
    WHERE UserId = @userId AND AgencyId = @agencyId;
    
    SELECT 'Agency unassigned successfully' AS Message;
END; 