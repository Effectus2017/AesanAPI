-- =============================================
-- Stored Procedure: 102_InsertSatelliteSite
-- Descripción: Inserta una relación de sitio satélite
-- Reemplaza: 102_InsertSatelliteSchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[102_InsertSatelliteSite]
    @mainSiteId INT,
    @satelliteSiteId INT,
    @comment NVARCHAR(255) = NULL,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteSatellite
        (
        MainSiteId, SatelliteSiteId, AssignmentDate, Comment, IsActive, CreatedAt
        )
    VALUES
        (
            @mainSiteId, @satelliteSiteId, GETDATE(), @comment, @isActive, GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
END;
