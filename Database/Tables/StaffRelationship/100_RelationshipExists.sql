-- =============================================
-- Stored Procedure: 100_RelationshipExists
-- =============================================
-- Verifica si ya existe una relación entre dos empleados específicos
-- Retorna el número de filas afectadas (1 si existe, 0 si no existe)

CREATE OR ALTER PROCEDURE [dbo].[100_RelationshipExists]
    @staffId INT,
    @relatedStaffId INT,
    @relationshipTypeId INT = NULL,
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar que ambos empleados existan
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId)
        BEGIN
        RAISERROR ('El empleado principal no existe', 16, 1);
        RETURN;
    END
        
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @relatedStaffId)
        BEGIN
        RAISERROR ('El empleado relacionado no existe', 16, 1);
        RETURN;
    END
        
        -- Verificar si ya existe una relación entre estos empleados
        DECLARE @exists BIT = 0;
        
        IF @relationshipTypeId IS NULL
        BEGIN
        -- Verificar cualquier tipo de relación
        SELECT @exists = CASE WHEN EXISTS (
                SELECT 1
            FROM StaffRelationship
            WHERE (StaffId = @staffId AND RelatedStaffId = @relatedStaffId)
                OR (StaffId = @relatedStaffId AND RelatedStaffId = @staffId)
                AND IsActive = 1
            ) THEN 1 ELSE 0 END;
    END
        ELSE
        BEGIN
        -- Verificar un tipo específico de relación
        SELECT @exists = CASE WHEN EXISTS (
                SELECT 1
            FROM StaffRelationship
            WHERE (StaffId = @staffId AND RelatedStaffId = @relatedStaffId)
                OR (StaffId = @relatedStaffId AND RelatedStaffId = @staffId)
                AND RelationshipTypeId = @relationshipTypeId
                AND IsActive = 1
            ) THEN 1 ELSE 0 END;
    END
        
        -- Establecer el parámetro de salida
        SET @rowsAffected = @exists;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
