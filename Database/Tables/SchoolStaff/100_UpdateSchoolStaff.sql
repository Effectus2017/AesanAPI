-- =============================================
-- Stored Procedure: 100_UpdateSchoolStaff
-- Descripción: Actualiza una asignación SchoolStaff por Id
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchoolStaff]
    @id INT,
    @isPrimary BIT,
    @startDate DATE = NULL,
    @endDate DATE = NULL,
    @comments NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @isPrimary = 1
    BEGIN
        DECLARE @schoolId INT;
        SELECT @schoolId = SchoolId FROM SchoolStaff WHERE Id = @id AND IsActive = 1;

        IF @schoolId IS NOT NULL
        BEGIN
            UPDATE SchoolStaff
            SET IsPrimary = 0, UpdatedAt = GETDATE()
            WHERE SchoolId = @schoolId AND IsActive = 1 AND Id <> @id;
        END
    END

    UPDATE SchoolStaff
    SET
        IsPrimary = @isPrimary,
        StartDate = @startDate,
        EndDate = @endDate,
        Comments = @comments,
        UpdatedAt = GETDATE()
    WHERE Id = @id AND IsActive = 1;

    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Result;
END;
