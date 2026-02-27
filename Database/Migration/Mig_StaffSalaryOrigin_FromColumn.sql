-- =============================================
-- Migración: StaffSalaryOrigin desde columna Staff.SalaryOriginIds
-- Descripción: Copia los datos de Staff.SalaryOriginIds a la tabla StaffSalaryOrigin.
-- Ejecutar después de: StaffSalaryOrigin-Table.sql (crear tabla).
-- La columna Staff.SalaryOriginIds no se elimina en esta migración (opcional en una posterior).
-- =============================================

SET NOCOUNT ON;

BEGIN TRY
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StaffSalaryOrigin' AND schema_id = SCHEMA_ID('dbo'))
    BEGIN
        RAISERROR('La tabla StaffSalaryOrigin no existe. Ejecutar primero StaffSalaryOrigin-Table.sql', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Staff]') AND name = 'SalaryOriginIds')
    BEGIN
        PRINT 'La columna Staff.SalaryOriginIds no existe. No hay datos que migrar.';
        RETURN;
    END

    BEGIN TRANSACTION;

    -- Inserción set-based: por cada Staff con SalaryOriginIds no vacío, insertar en StaffSalaryOrigin
    -- solo los IDs que existan en OptionSelection con OptionKey = ''salaryOrigin'' (evitando duplicados por DISTINCT)
    INSERT INTO StaffSalaryOrigin
        (StaffId, OptionSelectionId, IsActive, CreatedAt)
    SELECT DISTINCT
        s.Id,
        os.Id,
        1,
        GETDATE()
    FROM Staff s
        CROSS APPLY STRING_SPLIT(NULLIF(LTRIM(RTRIM(s.SalaryOriginIds)), ''), ',') ids
        INNER JOIN OptionSelection os ON os.Id = TRY_CAST(LTRIM(RTRIM(ids.value)) AS INT)
            AND os.OptionKey = 'salaryOrigin'
            AND os.IsActive = 1
    WHERE s.SalaryOriginIds IS NOT NULL
        AND LTRIM(RTRIM(s.SalaryOriginIds)) <> '';

    DECLARE @InsertedRows INT = @@ROWCOUNT;
    COMMIT TRANSACTION;

    PRINT 'Migración StaffSalaryOrigin completada. Filas insertadas: ' + CAST(@InsertedRows AS NVARCHAR(10));
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    PRINT 'Error en migración StaffSalaryOrigin: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO
