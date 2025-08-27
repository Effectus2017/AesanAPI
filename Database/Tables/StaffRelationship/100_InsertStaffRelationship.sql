-- =============================================
-- Stored Procedure: InsertStaffRelationship
-- =============================================
-- Inserta una nueva relación de parentesco entre empleados
-- Incluye validaciones de negocio

CREATE OR ALTER PROCEDURE [dbo].[100_InsertStaffRelationship]
    @staffId INT,
    @relatedStaffId INT,
    @relationshipTypeId INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar que los empleados existan y estén activos
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId AND IsActive = 1)
        BEGIN
        RAISERROR ('El empleado principal no existe o no está activo', 16, 1);
        RETURN;
    END
        
        IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @relatedStaffId AND IsActive = 1)
        BEGIN
        RAISERROR ('El empleado relacionado no existe o no está activo', 16, 1);
        RETURN;
    END
        
        -- Validar que no sea la misma persona
        IF @staffId = @relatedStaffId
        BEGIN
        RAISERROR ('Un empleado no puede tener parentesco consigo mismo', 16, 1);
        RETURN;
    END
        
        -- Validar que el tipo de relación sea válido
        IF NOT EXISTS (SELECT 1
    FROM OptionSelection
    WHERE Id = @relationshipTypeId AND OptionKey = 'staffRelationshipType' AND IsActive = 1)
        BEGIN
        RAISERROR ('El tipo de parentesco no es válido', 16, 1);
        RETURN;
    END
        
        -- Verificar si ya existe una relación entre estos empleados
        IF EXISTS (SELECT 1
    FROM StaffRelationship
    WHERE ((StaffId = @staffId AND RelatedStaffId = @relatedStaffId)
        OR (StaffId = @relatedStaffId AND RelatedStaffId = @staffId))
        AND IsActive = 1)
        BEGIN
        RAISERROR ('Ya existe una relación activa entre estos empleados', 16, 1);
        RETURN;
    END
        
        -- Para esposo(a), verificar que no tenga otro esposo(a) activo
        DECLARE @relationshipType NVARCHAR(100);
        SELECT @relationshipType = Name
    FROM OptionSelection
    WHERE Id = @relationshipTypeId;
        
        IF @relationshipType LIKE '%Esposo%'
        BEGIN
        IF EXISTS (SELECT 1
        FROM StaffRelationship sr
            INNER JOIN OptionSelection rt ON sr.RelationshipTypeId = rt.Id
        WHERE (sr.StaffId = @staffId OR sr.RelatedStaffId = @staffId)
            AND rt.Name LIKE '%Esposo%' AND sr.IsActive = 1)
            BEGIN
            RAISERROR ('El empleado ya tiene un esposo(a) activo', 16, 1);
            RETURN;
        END
    END
        
        -- Insertar la relación
        INSERT INTO StaffRelationship
        (StaffId, RelatedStaffId, RelationshipTypeId, IsActive, CreatedAt)
    VALUES
        (@staffId, @relatedStaffId, @relationshipTypeId, 1, GETDATE());
        
        -- Establecer el parámetro de salida
        SET @id = SCOPE_IDENTITY();
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
