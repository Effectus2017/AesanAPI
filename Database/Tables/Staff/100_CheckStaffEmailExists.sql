-- =============================================
-- Stored Procedure: 100_CheckStaffEmailExists
-- =============================================
-- Verifica si un correo electrónico ya existe en la tabla Staff
-- Parámetros:
--   @email: El correo electrónico a verificar
-- Retorna:
--   @exists: 1 si el correo existe, 0 si no existe

CREATE OR ALTER PROCEDURE [dbo].[100_CheckStaffEmailExists]
    @email NVARCHAR(255),
    @exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Verificar si el correo existe (case-insensitive)
        IF EXISTS (
            SELECT 1
    FROM Staff
    WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@email)))
        AND Email IS NOT NULL
        AND Email != ''
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

