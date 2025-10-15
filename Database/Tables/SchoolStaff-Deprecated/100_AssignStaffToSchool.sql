-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 100_AssignStaffToSite
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================
-- Stored Procedure: 100_AssignStaffToSchool
-- =============================================
-- Asigna un empleado a un sitio específico
-- Incluye validaciones para asegurar la integridad de los datos

CREATE OR ALTER PROCEDURE [dbo].[100_AssignStaffToSchool]
    @schoolId INT,
    @staffId INT,
    @assignmentTypeId INT,
    @isPrimary BIT = 0,
    @startDate DATE = NULL,
    @endDate DATE = NULL,
    @comments NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que el sitio existe y está activo
        IF NOT EXISTS (SELECT 1
    FROM School
    WHERE Id = @schoolId AND IsActive = 1)
        BEGIN
        RAISERROR ('El sitio especificado no existe o no está activo', 16, 1);
        RETURN;
    END
        
        -- Verificar que el empleado existe y está activo
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('El empleado especificado no existe o no está activo', 16, 1);
        RETURN;
    END
        
        -- Verificar que el empleado es de tipo "Empleado" (StaffTypeId = 1)
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId AND StaffTypeId = 1)
        BEGIN
        RAISERROR ('Solo se pueden asignar empleados (StaffTypeId = 1) a sitios', 16, 1);
        RETURN;
    END
        
        -- Verificar que el tipo de asignación existe
        IF NOT EXISTS (SELECT 1
    FROM OptionSelection
    WHERE Id = @assignmentTypeId AND OptionKey = 'staffAssignmentType' AND IsActive = 1)
        BEGIN
        RAISERROR ('El tipo de asignación especificado no existe o no está activo', 16, 1);
        RETURN;
    END
        
        -- Verificar que no existe una asignación activa del mismo empleado al mismo sitio
        IF EXISTS (SELECT 1
    FROM SchoolStaff
    WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('El empleado ya está asignado activamente a este sitio', 16, 1);
        RETURN;
    END
        
        -- Si es asignación principal, desactivar otras asignaciones principales del sitio
        IF @isPrimary = 1
        BEGIN
        UPDATE SchoolStaff 
            SET IsPrimary = 0, UpdatedAt = GETDATE()
            WHERE SchoolId = @schoolId AND IsActive = 1;
    END
        
        -- Insertar la nueva asignación
        INSERT INTO SchoolStaff
        (
        SchoolId, StaffId, AssignmentDate, AssignmentTypeId,
        IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt
        )
    VALUES
        (
            @schoolId, @staffId, GETDATE(), @assignmentTypeId,
            @isPrimary, @startDate, @endDate, @comments, 1, GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        -- Retornar el ID de la nueva asignación
        SELECT SCOPE_IDENTITY() AS Id;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
