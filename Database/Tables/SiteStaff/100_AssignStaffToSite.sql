-- =============================================
-- Stored Procedure: 100_AssignStaffToSite
-- Descripción: Asigna un empleado a un sitio específico
-- Reemplaza: 100_AssignStaffToSchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================
-- Asigna un empleado a un sitio específico
-- Incluye validaciones para asegurar la integridad de los datos
-- Devuelve el ID de la asignación creada mediante SELECT

CREATE OR ALTER PROCEDURE [dbo].[100_AssignStaffToSite]
    @siteId INT,
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
        
        DECLARE @assignmentTypeId INT = 1; -- Valor por defecto
        DECLARE @isActive BIT = 1; -- Valor por defecto
        
        -- Verificar que el sitio existe y está activo
        IF NOT EXISTS (SELECT 1
    FROM Site
    WHERE Id = @siteId AND IsActive = 1)
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
        
        -- Verificar que no existe una asignación activa del mismo empleado al mismo sitio
        IF EXISTS (SELECT 1
    FROM SiteStaff
    WHERE SiteId = @siteId AND StaffId = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('El empleado ya está asignado activamente a este sitio', 16, 1);
        RETURN;
    END
        
        -- Si es asignación principal, desactivar otras asignaciones principales del sitio
        IF @isPrimary = 1
        BEGIN
        UPDATE SiteStaff 
                SET IsPrimary = 0, UpdatedAt = GETDATE()
                WHERE SiteId = @siteId AND IsActive = 1;
    END
        
        -- Insertar la nueva asignación
        INSERT INTO SiteStaff
        (
        SiteId, StaffId, AssignmentDate, AssignmentTypeId,
        IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt
        )
    VALUES
        (
            @siteId, @staffId, GETDATE(), @assignmentTypeId,
            @isPrimary, @startDate, @endDate, @comments, @isActive, GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        -- Retornar el ID de la nueva asignación mediante SELECT para QuerySingleAsync<int>
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

