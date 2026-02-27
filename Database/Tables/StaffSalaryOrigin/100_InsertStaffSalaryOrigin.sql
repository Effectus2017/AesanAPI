-- =============================================
-- Stored Procedure: 100_InsertStaffSalaryOrigin
-- Descripción: Inserta un origen de salario para un staff (una fila)
-- Parámetros lowercase; cuerpo con columnas CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertStaffSalaryOrigin]
    @staffid INT,
    @optionselectionid INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el ID exista en OptionSelection con OptionKey = 'salaryOrigin'
    IF NOT EXISTS (
        SELECT 1
        FROM OptionSelection
        WHERE Id = @optionselectionid
            AND OptionKey = 'salaryOrigin'
            AND IsActive = 1
    )
    BEGIN
        RAISERROR('El OptionSelectionId no existe o no tiene OptionKey = ''salaryOrigin''.', 16, 1);
        RETURN;
    END

    INSERT INTO StaffSalaryOrigin
        (StaffId, OptionSelectionId, IsActive, CreatedAt)
    VALUES
        (@staffid, @optionselectionid, 1, GETDATE());
END;
GO
