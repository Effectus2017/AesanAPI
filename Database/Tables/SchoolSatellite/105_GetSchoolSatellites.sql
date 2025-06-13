SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para obtener todas las relaciones SchoolSatellite con filtros y paginación
CREATE OR ALTER PROCEDURE [dbo].[105_GetSchoolSatellites]
    @take INT = 25,
    @skip INT = 0,
    @mainSchoolId INT = NULL,
    @satelliteSchoolId INT = NULL,
    @status NVARCHAR(50) = NULL,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.MainSchoolId,
        ms.Name AS MainSchoolName,
        ss.SatelliteSchoolId,
        sat.Name AS SatelliteSchoolName,
        ss.AssignmentDate,
        ss.Status,
        ss.Comment,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt
    FROM SchoolSatellite ss
        INNER JOIN School ms ON ss.MainSchoolId = ms.Id
        INNER JOIN School sat ON ss.SatelliteSchoolId = sat.Id
    WHERE (@mainSchoolId IS NULL OR ss.MainSchoolId = @mainSchoolId)
        AND (@satelliteSchoolId IS NULL OR ss.SatelliteSchoolId = @satelliteSchoolId)
        AND (@status IS NULL OR ss.Status = @status)
        AND (@isActive IS NULL OR ss.IsActive = @isActive)
    ORDER BY ss.Id
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Total count
    SELECT COUNT(*) AS TotalCount
    FROM SchoolSatellite ss
    WHERE (@mainSchoolId IS NULL OR ss.MainSchoolId = @mainSchoolId)
        AND (@satelliteSchoolId IS NULL OR ss.SatelliteSchoolId = @satelliteSchoolId)
        AND (@status IS NULL OR ss.Status = @status)
        AND (@isActive IS NULL OR ss.IsActive = @isActive);
END;
GO 