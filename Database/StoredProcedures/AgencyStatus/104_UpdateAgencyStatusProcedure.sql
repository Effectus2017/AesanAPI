-- =============================================
-- Author:      Development Team
-- Create date: 2025-03-07
-- Description: Script para actualizar el procedimiento almacenado que actualiza el estado de una agencia
-- =============================================

-- Actualizamos el procedimiento almacenado para actualizar el estado de una agencia
CREATE OR ALTER PROCEDURE [dbo].[104_UpdateAgencyStatus]
    @agencyId INT,
    @statusId INT,
    @rejectionJustification NVARCHAR(MAX) = NULL
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

    -- Si el estado es válido, actualizamos la agencia
    IF @validStatus = 1
    BEGIN
        -- Actualizamos el estado de la agencia
        UPDATE Agency
        SET 
            AgencyStatusId = @statusId,
            RejectionJustification = CASE 
                                        WHEN @statusId = 6 THEN @rejectionJustification -- 'No cumple con los requisitos'
                                        ELSE NULL
                                    END,
            UpdatedAt = GETDATE()
        WHERE Id = @agencyId;

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