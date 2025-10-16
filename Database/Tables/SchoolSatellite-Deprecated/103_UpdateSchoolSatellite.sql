-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 103_UpdateSiteSatellite
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para actualizar o insertar una relación entre escuela principal y satélite
CREATE OR ALTER PROCEDURE [dbo].[103_UpdateSchoolSatellite]
    @mainSchoolId INT,
    @satelliteSchoolId INT,
    @comment NVARCHAR(255) = NULL,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;
    DECLARE @recordExists INT;

    -- Verificar si ya existe la relación
    SELECT @recordExists = COUNT(1)
    FROM SchoolSatellite
    WHERE SatelliteSchoolId = @satelliteSchoolId;

    IF @recordExists > 0
    BEGIN
        -- Si existe, actualizar
        UPDATE SchoolSatellite
        SET MainSchoolId = @mainSchoolId,
            AssignmentDate = GETDATE(),
            Comment = @comment,
            IsActive = @isActive,
            UpdatedAt = GETDATE()
        WHERE SatelliteSchoolId = @satelliteSchoolId;

        SET @rowsAffected = @@ROWCOUNT;
    END
    ELSE
    BEGIN
        -- Si no existe, insertar
        INSERT INTO SchoolSatellite
            (MainSchoolId, SatelliteSchoolId, AssignmentDate, Comment, IsActive, CreatedAt)
        VALUES
            (@mainSchoolId, @satelliteSchoolId, GETDATE(), @comment, @isActive, GETDATE());

        SET @rowsAffected = @@ROWCOUNT;
    END

    RETURN @rowsAffected;
END;
GO 