-- =============================================
-- SP: 100_DeleteSiteVisit — borrado lógico
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteVisit]
    @id INT,
    @agencyid INT,
    @userid NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[SiteVisit]
    SET
        [IsDeleted] = 1,
        [UpdatedAt] = GETDATE(),
        [UpdatedBy] = @userid
    WHERE [Id] = @id
      AND [IsDeleted] = 0
      AND EXISTS (
          SELECT 1 FROM [dbo].[Site] s
          WHERE s.Id = [SiteVisit].[SiteId] AND s.AgencyId = @agencyid
      );
END
GO
