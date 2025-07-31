-- =============================================
-- DEPRECATED: Este stored procedure está deprecado. Use Staff en su lugar.
-- Será eliminado en una versión futura.
-- =============================================
-- Stored Procedure: 100_UpdateEmployeeActiveStatus
-- =============================================
-- Actualiza el estado activo de un empleado
-- Parámetros:
--   @employeeId: ID del empleado
--   @isActive: Nuevo estado activo
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateEmployeeActiveStatus]
    @id INT,
    @isActive BIT,
    @inactiveJustification NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Validación para inactivación
    IF @isActive = 0 AND (@inactiveJustification IS NULL OR LTRIM(RTRIM(@inactiveJustification)) = '')
    BEGIN
        RAISERROR ('Se requiere justificación para inactivar el empleado', 16, 1);
        RETURN -1;
    END

    -- Actualizar estado
    IF @isActive = 1
    BEGIN
        UPDATE Employee
        SET IsActive = @isActive,
            UpdatedAt = GETDATE()
        WHERE Id = @id;
    END
    ELSE
    BEGIN
        UPDATE Employee
        SET IsActive = @isActive,
            Comments = ISNULL(Comments, '') + CHAR(13) + CHAR(10) + 'Inactivado: ' + @inactiveJustification,
            UpdatedAt = GETDATE()
        WHERE Id = @id;
    END

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END 