SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para obtener una relación SchoolSatellite por Id
CREATE OR ALTER PROCEDURE [dbo].[104_GetSchoolSatelliteById]
    @Id INT
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
    WHERE ss.Id = @Id;
END;
GO 