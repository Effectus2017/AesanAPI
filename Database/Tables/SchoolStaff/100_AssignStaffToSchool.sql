-- =============================================
-- Stored Procedure: 100_AssignStaffToSchool
-- Descripción: Asigna un empleado a una escuela
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_AssignStaffToSchool]
    @schoolId INT,
    @staffId INT,
    @isPrimary BIT = 0,
    @startDate DATE = NULL,
    @endDate DATE = NULL,
    @comments NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @assignmentTypeId INT = 1;
        DECLARE @isActive BIT = 1;

        IF NOT EXISTS (SELECT 1 FROM School WHERE Id = @schoolId AND IsActive = 1)
        BEGIN
            RAISERROR ('La escuela especificada no existe o no está activa', 16, 1);
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM Staff WHERE Id = @staffId AND IsActive = 1)
        BEGIN
            RAISERROR ('El empleado especificado no existe o no está activo', 16, 1);
            RETURN;
        END

        IF EXISTS (
            SELECT 1 FROM SchoolStaff
            WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1
        )
        BEGIN
            RAISERROR ('El empleado ya está asignado activamente a esta escuela', 16, 1);
            RETURN;
        END

        IF @isPrimary = 1
        BEGIN
            UPDATE SchoolStaff
            SET IsPrimary = 0, UpdatedAt = GETDATE()
            WHERE SchoolId = @schoolId AND IsActive = 1;
        END

        INSERT INTO SchoolStaff (
            SchoolId, StaffId, AssignmentDate, AssignmentTypeId,
            IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt
        )
        VALUES (
            @schoolId, @staffId, GETDATE(), @assignmentTypeId,
            @isPrimary, @startDate, @endDate, @comments, @isActive, GETDATE()
        );

        COMMIT TRANSACTION;

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
