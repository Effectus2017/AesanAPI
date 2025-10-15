-- =============================================
-- Stored Procedure: 103_UpdateSiteSatellite
-- Descripción: Actualiza o inserta una relación de sitio satélite
-- Reemplaza: 103_UpdateSchoolSatellite
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[103_UpdateSiteSatellite]
    @mainSiteId INT,
    @satelliteSiteId INT,
    @comment NVARCHAR(255) = NULL,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Verificar si ya existe la relación
        IF EXISTS (SELECT 1
    FROM SiteSatellite
    WHERE SatelliteSiteId = @satelliteSiteId)
        BEGIN
        -- Actualizar relación existente
        UPDATE SiteSatellite 
            SET 
                MainSiteId = @mainSiteId,
                Comment = @comment,
                IsActive = @isActive,
                UpdatedAt = GETDATE()
            WHERE SatelliteSiteId = @satelliteSiteId;
    END
        ELSE
        BEGIN
        -- Insertar nueva relación
        INSERT INTO SiteSatellite
            (
            MainSiteId, SatelliteSiteId, AssignmentDate, Comment, IsActive, CreatedAt
            )
        VALUES
            (
                @mainSiteId, @satelliteSiteId, GETDATE(), @comment, @isActive, GETDATE()
            );
    END

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
