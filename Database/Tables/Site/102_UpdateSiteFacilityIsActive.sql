CREATE OR ALTER PROCEDURE [dbo].[102_UpdateSiteFacilityIsActive]
    @site_id INT,
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SiteFacility
    SET IsActive = @is_active
    WHERE SiteId = @site_id;
END
GO
