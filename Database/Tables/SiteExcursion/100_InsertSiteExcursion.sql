-- =============================================
-- Stored Procedure: 100_InsertSiteExcursion
-- Descripción: Inserta una nueva excursión en la base de datos
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteExcursion]
    @siteId INT,
    @childGroupId INT = NULL,
    @activityDescription NVARCHAR(500),
    @excursionDate DATE,
    @isFullDay BIT = 0,
    @isUnforeseen BIT = 0,
    @comment NVARCHAR(1000) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteExcursion
        (
        SiteId, ChildGroupId, ActivityDescription, ExcursionDate,
        IsFullDay, IsUnforeseen, Comment, IsActive, CreatedAt
        )
    VALUES
        (
            @siteId, @childGroupId, @activityDescription, @excursionDate,
            @isFullDay, @isUnforeseen, @comment, 1, GETDATE()
        );

    SET @id = SCOPE_IDENTITY();
END;

