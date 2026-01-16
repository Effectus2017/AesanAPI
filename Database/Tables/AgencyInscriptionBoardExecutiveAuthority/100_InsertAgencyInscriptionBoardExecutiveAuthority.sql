-- =============================================
-- Stored Procedure: 100_InsertAgencyInscriptionBoardExecutiveAuthority
-- Descripción: Inserta múltiples funciones de autoridad de la Junta de Directores para una inscripción de agencia
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertAgencyInscriptionBoardExecutiveAuthority]
    @agencyInscriptionId INT,
    @optionSelectionIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear tabla temporal para los IDs
    DECLARE @OptionSelectionTable TABLE (OptionSelectionId INT);

    -- Insertar IDs en la tabla temporal
    INSERT INTO @OptionSelectionTable
        (OptionSelectionId)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@optionSelectionIds, ',')
    WHERE value IS NOT NULL AND value != '';

    -- Validar que todos los IDs existan en OptionSelection con OptionKey = 'boardExecutiveAuthority'
    IF EXISTS (
        SELECT 1
        FROM @OptionSelectionTable ost
        WHERE NOT EXISTS (
            SELECT 1
            FROM OptionSelection os
            WHERE os.Id = ost.OptionSelectionId
                AND os.OptionKey = 'boardExecutiveAuthority'
                AND os.IsActive = 1
        )
    )
    BEGIN
        RAISERROR('Uno o más OptionSelectionId no existen o no tienen OptionKey = ''boardExecutiveAuthority''.', 16, 1);
        RETURN;
    END

    -- Insertar funciones de autoridad
    INSERT INTO AgencyInscriptionBoardExecutiveAuthority
        (AgencyInscriptionId, OptionSelectionId, IsActive, CreatedAt)
    SELECT @agencyInscriptionId, OptionSelectionId, 1, GETDATE()
    FROM @OptionSelectionTable;
END;
GO
