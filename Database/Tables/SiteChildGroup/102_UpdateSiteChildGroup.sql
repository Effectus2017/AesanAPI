-- =============================================
-- Stored Procedure: 102_UpdateSiteChildGroup
-- Descripción: Actualiza nombre y número de niños de un grupo del sitio.
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteChildGroup]
    @id INT,
    @siteId INT,
    @groupName NVARCHAR(255),
    @numberOfChildren INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SiteChildGroup
    SET
        GroupName = @groupName,
        NumberOfChildren = @numberOfChildren,
        UpdatedAt = GETDATE()
    WHERE Id = @id
      AND SiteId = @siteId;
END;
