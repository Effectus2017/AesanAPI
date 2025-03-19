CREATE OR ALTER PROCEDURE [dbo].[101_UnassignAgencyToUser]
    @userId NVARCHAR(450),
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;
    
    DELETE FROM AgencyUsers
    WHERE UserId = @userId AND AgencyId = @agencyId AND IsMonitor = 1;
    
    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END; 