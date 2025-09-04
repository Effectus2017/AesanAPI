-- =============================================
-- Stored Procedure: 100_UpdateStaffImage
-- =============================================
-- Actualiza solo la imagen del staff en la base de datos
-- 
-- Este SP es independiente y se usa específicamente para
-- gestionar la imagen/avatar del personal
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffImage]
    @staffId INT,
    @imageURL NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el staff existe
    IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId)
    BEGIN
        RAISERROR ('El staff con ID %d no existe.', 16, 1, @staffId);
        RETURN -1;
    END

    -- Actualizar solo la imagen del staff
    UPDATE Staff 
    SET 
        ImageURL = @imageURL,
        UpdatedAt = GETDATE()
    WHERE Id = @staffId;

    -- Retornar el número de filas afectadas
    DECLARE @rowsAffected INT = @@ROWCOUNT;

    -- Log de la operación
    IF @rowsAffected > 0
    BEGIN
        PRINT 'Imagen del staff con ID ' + CAST(@staffId AS VARCHAR) + ' actualizada exitosamente.';

        -- Mostrar información del staff actualizado
        SELECT
            'Staff actualizado' as Status,
            Id,
            FirstName + ' ' + FatherLastName as FullName,
            ImageURL,
            UpdatedAt
        FROM Staff
        WHERE Id = @staffId;
    END
    ELSE
    BEGIN
        PRINT 'No se pudo actualizar la imagen del staff con ID ' + CAST(@staffId AS VARCHAR) + '.';
    END

    RETURN @rowsAffected;
END
