-- =============================================
-- Author:      Development Team
-- Create date: 2025-03-07
-- Description: Script para crear un procedimiento almacenado que actualice el orden de visualización de un estado de agencia
-- =============================================

-- Creamos el procedimiento almacenado para actualizar el orden de visualización de un estado de agencia
CREATE OR ALTER PROCEDURE [dbo].[105_UpdateAgencyStatusDisplayOrder]
    @statusId INT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;
    DECLARE @validStatus BIT = 0;

    -- Verificamos que el estado exista
    IF EXISTS (SELECT 1 FROM AgencyStatus WHERE Id = @statusId)
    BEGIN
        SET @validStatus = 1;
    END

    -- Si el estado es válido, actualizamos su orden de visualización
    IF @validStatus = 1
    BEGIN
        -- Actualizamos el orden de visualización del estado
        UPDATE AgencyStatus
        SET 
            DisplayOrder = @displayOrder,
            UpdatedAt = GETDATE()
        WHERE Id = @statusId;

        -- Obtenemos el número de filas afectadas
        SET @rowsAffected = @@ROWCOUNT;

        -- Retornamos el número de filas afectadas
        RETURN @rowsAffected;
    END
    ELSE
    BEGIN
        -- Si el estado no es válido, retornamos 0
        RETURN 0;
    END
END; 