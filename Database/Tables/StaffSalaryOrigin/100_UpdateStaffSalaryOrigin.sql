-- =============================================
-- Stored Procedure: 100_UpdateStaffSalaryOrigin
-- Descripción: Actualiza múltiples orígenes de salario para un staff (reemplaza los existentes)
-- Parámetros lowercase; cuerpo con columnas CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffSalaryOrigin]
    @staffid INT,
    @optionselectionids NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear tabla temporal para los IDs
    DECLARE @OptionSelectionTable TABLE (OptionSelectionId INT);

    -- Insertar IDs en la tabla temporal
    INSERT INTO @OptionSelectionTable
        (OptionSelectionId)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@optionselectionids, ',')
    WHERE value IS NOT NULL AND value != '';

    -- Validar que todos los IDs existan en OptionSelection con OptionKey = 'salaryOrigin'
    IF EXISTS (
        SELECT 1
        FROM @OptionSelectionTable ost
        WHERE NOT EXISTS (
            SELECT 1
            FROM OptionSelection os
            WHERE os.Id = ost.OptionSelectionId
                AND os.OptionKey = 'salaryOrigin'
                AND os.IsActive = 1
        )
    )
    BEGIN
        RAISERROR('Uno o más OptionSelectionId no existen o no tienen OptionKey = ''salaryOrigin''.', 16, 1);
        RETURN;
    END

    -- Eliminar orígenes de salario existentes
    DELETE FROM StaffSalaryOrigin
    WHERE StaffId = @staffid;

    -- Insertar nuevos orígenes de salario (si hay alguno)
    IF EXISTS (SELECT 1 FROM @OptionSelectionTable)
    BEGIN
        INSERT INTO StaffSalaryOrigin
            (StaffId, OptionSelectionId, IsActive, CreatedAt)
        SELECT @staffid, OptionSelectionId, 1, GETDATE()
        FROM @OptionSelectionTable;
    END
END;
GO
