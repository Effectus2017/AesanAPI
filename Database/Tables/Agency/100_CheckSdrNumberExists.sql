-- =============================================
-- Stored Procedure: 100_CheckSdrNumberExists
-- =============================================
-- Verifica si un SDR (Número de Registro del Departamento de Estado) ya existe en la tabla Agency
-- Parámetros:
--   @sdrNumber: El número SDR a verificar
-- Retorna:
--   @exists: 1 si el SDR existe, 0 si no existe

CREATE OR ALTER PROCEDURE [dbo].[100_CheckSdrNumberExists]
    @sdrNumber BIGINT,
    @exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Verificar si el SDR existe
        IF EXISTS (
            SELECT 1
            FROM Agency
            WHERE SdrNumber = @sdrNumber
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

