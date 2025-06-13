SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para insertar una relación entre escuela principal y satélite
CREATE OR ALTER PROCEDURE [dbo].[101_InsertSchoolSatellite]
    @MainSchoolId INT,
    @SatelliteSchoolId INT,
    @AssignmentDate DATE = NULL,
    @Status NVARCHAR(50) = NULL,
    @Comment NVARCHAR(255) = NULL,
    @IsActive BIT = 1,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SchoolSatellite
        (
        MainSchoolId, SatelliteSchoolId, AssignmentDate, Status, Comment, IsActive, CreatedAt
        )
    VALUES
        (
            @MainSchoolId, @SatelliteSchoolId, @AssignmentDate, @Status, @Comment, @IsActive, GETDATE()
    );

    SET @Id = SCOPE_IDENTITY();
    RETURN @Id;
END;
GO 