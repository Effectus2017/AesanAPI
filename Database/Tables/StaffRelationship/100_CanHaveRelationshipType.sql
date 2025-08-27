-- =============================================
-- Stored Procedure: 100_CanHaveRelationshipType
-- =============================================
-- Verifica si un empleado puede tener el tipo de relación especificado
-- Retorna el número de filas afectadas (1 si puede, 0 si no puede)

CREATE OR ALTER PROCEDURE [dbo].[100_CanHaveRelationshipType]
    @staffId INT,
    @relationshipTypeId INT,
    @rowsAffected INT OUTPUT
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
        IF EXISTS (
            SELECT 1
    FROM StaffRelationship
    WHERE StaffId = @staffId
        AND RelationshipTypeId = @relationshipTypeId
        AND IsActive = 1
        )
        BEGIN
        -- Ya tiene una relación de este tipo
        SET @canHave = 0;
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
