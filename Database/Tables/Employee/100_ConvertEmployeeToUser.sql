-- =============================================
-- DEPRECATED: Este stored procedure está deprecado. Use Staff en su lugar.
-- Será eliminado en una versión futura.
-- =============================================
-- Stored Procedure: 100_ConvertEmployeeToUser
-- =============================================
-- Convierte un empleado en usuario del sistema
-- Parámetros:
--   @employeeId: ID del empleado
--   @userId: ID del usuario
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ConvertEmployeeToUser]
    @employeeId INT,
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Employee
    SET
        UserId = @userId,
        UpdatedAt = GETDATE()
    WHERE Id = @employeeId;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END 