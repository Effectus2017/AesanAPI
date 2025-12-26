-- =============================================
-- Stored Procedure: 100_CheckEinNumberExists
-- =============================================
-- Verifica si un EIN (Número de Seguro Social Patronal) ya existe en la tabla Agency
-- Parámetros:
--   @einNumber: El número EIN a verificar
-- Retorna:
--   @exists: 1 si el EIN existe, 0 si no existe

CREATE OR ALTER PROCEDURE [dbo].[100_CheckEinNumberExists]
    @einNumber INT,
    @exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Verificar si el EIN existe
        IF EXISTS (
            SELECT 1
            FROM Agency
            WHERE EinNumber = @einNumber
        )
        BEGIN
            SET @exists = 1;
        END
        ELSE
        BEGIN
            SET @exists = 0;
        END
    END TRY
    BEGIN CATCH
        -- En caso de error, retornar 0 (no existe)
        SET @exists = 0;
        THROW;
    END CATCH
END

