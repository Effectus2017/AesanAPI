-- =============================================
-- Stored Procedure: 100_CheckUieNumberExists
-- =============================================
-- Verifica si un IUE (Identificador Único de Entidad) ya existe en la tabla Agency
-- Parámetros:
--   @uieNumber: El número IUE a verificar
-- Retorna:
--   @exists: 1 si el IUE existe, 0 si no existe

CREATE OR ALTER PROCEDURE [dbo].[100_CheckUieNumberExists]
    @uieNumber BIGINT,
    @exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Verificar si el IUE existe
        -- Comparación directa BIGINT con BIGINT
        IF EXISTS (
            SELECT 1
    FROM Agency
    WHERE UieNumber = @uieNumber
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

DECLARE @exists BIT;
EXEC [dbo].[100_CheckUieNumberExists] @uieNumber = 123456789012, @exists = @exists OUTPUT;
SELECT @exists;