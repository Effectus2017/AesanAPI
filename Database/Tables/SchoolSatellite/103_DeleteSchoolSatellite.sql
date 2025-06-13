SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para borrado lógico de una relación entre escuela principal y satélite
CREATE OR ALTER PROCEDURE [dbo].[103_DeleteSchoolSatellite]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE SchoolSatellite
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 