-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 100_UnassignStaffFromSite
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================
-- Stored Procedure: 100_UnassignStaffFromSchool
-- =============================================
-- Desasigna un empleado de un sitio específico
-- Realiza una baja lógica (marca como inactivo)

CREATE OR ALTER PROCEDURE [dbo].[100_UnassignStaffFromSchool]
    @schoolId INT,
    @staffId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la asignación existe y está activa
        IF NOT EXISTS (SELECT 1
    FROM SchoolStaff
    WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('La asignación especificada no existe o no está activa', 16, 1);
        RETURN;
    END
        
        -- Realizar baja lógica
        UPDATE SchoolStaff 
        SET IsActive = 0, UpdatedAt = GETDATE()
        WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1;
        
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
