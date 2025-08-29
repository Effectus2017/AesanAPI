-- =============================================
-- Stored Procedure: 100_UpdateStaffRelationship
-- =============================================
-- Actualiza una relación existente entre empleados
-- Incluye el campo Comment para registrar el motivo del cambio

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffRelationship]
    @id INT,
    @relationshipTypeId INT,
    @isActive BIT,
    @comment NVARCHAR(500) = NULL,
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la relación existe
    IF NOT EXISTS (SELECT 1
    FROM StaffRelationship
    WHERE Id = @id)
    BEGIN
        RAISERROR ('La relación con ID %d no existe', 16, 1, @id);
        RETURN;
    END

    -- Validar que el tipo de relación existe
    IF NOT EXISTS (SELECT 1
    FROM OptionSelection
    WHERE Id = @relationshipTypeId AND OptionKey = 'staffRelationshipType')
    BEGIN
        RAISERROR ('El tipo de relación con ID %d no es válido', 16, 1, @relationshipTypeId);
        RETURN;
    END

    -- Actualizar la relación
    UPDATE StaffRelationship 
    SET 
        RelationshipTypeId = @relationshipTypeId,
        IsActive = @isActive,
        Comment = @comment,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;

    IF @rowsAffected > 0
    BEGIN
        PRINT 'Relación actualizada exitosamente';
    END
    ELSE
    BEGIN
        PRINT 'No se pudo actualizar la relación';
    END
END
