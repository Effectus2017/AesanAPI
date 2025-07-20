-- =============================================
-- Stored Procedure: 100_ConvertEmployeeToUser
-- =============================================
-- Convierte un empleado en usuario del sistema
-- Asocia el UserId al empleado para indicar que ya tiene un usuario

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