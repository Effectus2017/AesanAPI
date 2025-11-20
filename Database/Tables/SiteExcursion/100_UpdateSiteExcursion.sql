-- =============================================
-- Stored Procedure: 100_UpdateSiteExcursion
-- Descripción: Actualiza una excursión existente en la base de datos
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteExcursion]
    @id INT,
    @siteId INT,
    @childGroupId INT = NULL,
    @activityDescription NVARCHAR(500),
    @excursionDate DATE,
    @isFullDay BIT = 0,
    @isUnforeseen BIT = 0,
    @comment NVARCHAR(1000) = NULL,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SiteExcursion
    SET SiteId = @siteId,
        ChildGroupId = @childGroupId,
        ActivityDescription = @activityDescription,
        ExcursionDate = @excursionDate,
        IsFullDay = @isFullDay,
        IsUnforeseen = @isUnforeseen,
        Comment = @comment,
        IsActive = @isActive,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END;

