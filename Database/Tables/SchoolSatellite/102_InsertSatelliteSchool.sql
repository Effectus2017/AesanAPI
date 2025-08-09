SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para insertar una relación entre escuela principal y satélite
CREATE OR ALTER PROCEDURE [dbo].[102_InsertSatelliteSchool]
    @mainSchoolId INT,
    @satelliteSchoolId INT,
    @assignmentDate DATE = NULL,
    @comment NVARCHAR(255) = NULL,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SchoolSatellite
        (MainSchoolId, SatelliteSchoolId, AssignmentDate, Comment, IsActive, CreatedAt, UpdatedAt)
    VALUES(@mainSchoolId, @satelliteSchoolId, @assignmentDate, @comment, @isActive, GETDATE(), GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO 