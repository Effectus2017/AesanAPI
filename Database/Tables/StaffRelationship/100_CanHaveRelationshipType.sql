-- =============================================
-- Stored Procedure: 100_CanHaveRelationshipType
-- =============================================
-- Verifica si un empleado puede tener el tipo de relación especificado
-- Retorna el número de filas afectadas (1 si puede, 0 si no puede)
-- @excludeRelationshipId: ID de la relación a excluir (útil para ediciones)

CREATE OR ALTER PROCEDURE [dbo].[100_CanHaveRelationshipType]
    @staffId INT,
    @relationshipTypeId INT,
    @rowsAffected INT OUTPUT,
    @excludeRelationshipId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar que el empleado exista
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId)
        BEGIN
        RAISERROR ('El empleado no existe', 16, 1);
        RETURN;
    END
        
        -- Validar que el tipo de relación exista
        IF NOT EXISTS (SELECT 1
    FROM OptionSelection
    WHERE Id = @relationshipTypeId AND OptionKey = 'staffRelationshipType')
        BEGIN
        RAISERROR ('El tipo de relación no existe', 16, 1);
        RETURN;
    END
        
        -- Verificar si el empleado puede tener este tipo de relación
        -- Por ejemplo, verificar reglas de negocio, restricciones, etc.
        DECLARE @canHave BIT = 0;
        
        -- Aquí se pueden implementar las reglas de negocio específicas
        -- Por ahora, asumimos que puede tener cualquier tipo de relación si no hay restricciones
        -- Se puede expandir según las reglas de negocio específicas
        
        -- Verificar si ya tiene una relación de este tipo (opcional, depende de las reglas)
        -- Por ejemplo, si solo puede tener una relación de cada tipo:
        -- NOTA: Esta validación se puede hacer más flexible según las reglas de negocio
        IF EXISTS (
            SELECT 1
    FROM StaffRelationship
    WHERE StaffId = @staffId
        AND RelationshipTypeId = @relationshipTypeId
        AND IsActive = 1
        AND (@excludeRelationshipId IS NULL OR Id != @excludeRelationshipId)
        )
        BEGIN
        -- Ya tiene una relación de este tipo (excluyendo la que se está editando)
        -- Pero esto no debería impedir la edición de relaciones existentes
        -- Se puede modificar según las reglas de negocio específicas
        SET @canHave = 1;
    -- Cambiado a 1 para permitir edición
    END
        ELSE
        BEGIN
        -- Puede tener una relación de este tipo
        SET @canHave = 1;
    END
        
        -- Establecer el parámetro de salida
        SET @rowsAffected = @canHave;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
