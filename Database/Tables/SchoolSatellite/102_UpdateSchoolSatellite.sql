SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para actualizar una relación entre escuela principal y satélite
CREATE OR ALTER PROCEDURE [dbo].[102_UpdateSchoolSatellite]
    @mainSchoolId INT,
    @satelliteSchoolId INT,
    @status NVARCHAR(50) = NULL,
    @comment NVARCHAR(255) = NULL,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE SchoolSatellite
    SET MainSchoolId = @mainSchoolId,
        SatelliteSchoolId = @satelliteSchoolId,
        AssignmentDate = GETDATE(),
        Status = @status,
        Comment = @comment,
        IsActive = @isActive,
        UpdatedAt = GETDATE()
    WHERE SatelliteSchoolId = @satelliteSchoolId;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 