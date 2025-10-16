-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 100_UpdateSiteStaff
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================
-- Stored Procedure: 100_UpdateSchoolStaff
-- =============================================
-- Actualiza una asignación existente entre un empleado y un sitio
-- Permite modificar el tipo de asignación, fechas y comentarios

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchoolStaff]
    @id INT,
    @assignmentTypeId INT = NULL,
    @isPrimary BIT = NULL,
    @startDate DATE = NULL,
    @endDate DATE = NULL,
    @comments NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la asignación existe y está activa
        IF NOT EXISTS (SELECT 1
    FROM SchoolStaff
    WHERE Id = @id AND IsActive = 1)
        BEGIN
        RAISERROR ('La asignación especificada no existe o no está activa', 16, 1);
        RETURN;
    END
        
        -- Si se va a cambiar a asignación principal, desactivar otras asignaciones principales del mismo sitio
        IF @isPrimary = 1
        BEGIN
        DECLARE @schoolId INT;
        SELECT @schoolId = SchoolId
        FROM SchoolStaff
        WHERE Id = @id;

        UPDATE SchoolStaff 
            SET IsPrimary = 0, UpdatedAt = GETDATE()
            WHERE SchoolId = @schoolId AND Id != @id AND IsActive = 1;
    END
        
        -- Verificar que el tipo de asignación existe (si se va a cambiar)
        IF @assignmentTypeId IS NOT NULL
        BEGIN
        IF NOT EXISTS (SELECT 1
        FROM OptionSelection
        WHERE Id = @assignmentTypeId AND OptionKey = 'staffAssignmentType' AND IsActive = 1)
            BEGIN
            RAISERROR ('El tipo de asignación especificado no existe o no está activo', 16, 1);
            RETURN;
        END
    END
        
        -- Actualizar la asignación
        UPDATE SchoolStaff 
        SET 
            AssignmentTypeId = ISNULL(@assignmentTypeId, AssignmentTypeId),
            IsPrimary = ISNULL(@isPrimary, IsPrimary),
            StartDate = ISNULL(@startDate, StartDate),
            EndDate = ISNULL(@endDate, EndDate),
            Comments = ISNULL(@comments, Comments),
            UpdatedAt = GETDATE()
        WHERE Id = @id AND IsActive = 1;
        
        COMMIT TRANSACTION;
        
        -- Retornar éxito
        SELECT 1 AS Success;
        
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
