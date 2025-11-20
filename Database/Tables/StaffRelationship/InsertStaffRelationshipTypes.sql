-- =============================================
-- Script: InsertStaffRelationshipTypes
-- =============================================
-- Limpia y reinserta todos los tipos de parentesco en OptionSelection
-- Este script debe ejecutarse después de crear la tabla StaffRelationship
-- 
-- IMPORTANTE: Este script eliminará todas las relaciones existentes en StaffRelationship
-- que usan estos tipos de parentesco. Ejecutar con precaución.

BEGIN TRANSACTION;

BEGIN TRY
    -- Paso 1: Eliminar todas las relaciones existentes que usan estos tipos
    -- Esto es necesario porque hay una restricción de clave foránea
    DELETE FROM StaffRelationship
    WHERE RelationshipTypeId IN (
        SELECT Id
FROM OptionSelection
WHERE OptionKey = 'staffRelationshipType'
    );

    -- Paso 2: Eliminar tipos de parentesco existentes
    DELETE FROM OptionSelection
    WHERE OptionKey = 'staffRelationshipType';

    -- Paso 3: Insertar todos los tipos de parentesco entre empleados (optionKey = 'staffRelationshipType')
    INSERT INTO OptionSelection
    (Name, NameEn, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Abuelo(a)', 'Grandfather/Grandmother', 'staffRelationshipType', 0, 1, 800),
    ('Esposo(a)', 'Spouse', 'staffRelationshipType', 0, 1, 810),
    ('Encargado(a) Legal', 'Legal Guardian', 'staffRelationshipType', 0, 1, 820),
    ('Hijo(a)', 'Child', 'staffRelationshipType', 0, 1, 830),
    ('Suegro(a)', 'Father/Mother-in-law', 'staffRelationshipType', 0, 1, 840),
    ('Padre', 'Father', 'staffRelationshipType', 0, 1, 850),
    ('Madre', 'Mother', 'staffRelationshipType', 0, 1, 860),
    ('Hermano(a)', 'Sibling', 'staffRelationshipType', 0, 1, 870),
    ('Cuñado(a)', 'Brother/Sister-in-law', 'staffRelationshipType', 0, 1, 880),
    ('Sobrino(a)', 'Nephew/Niece', 'staffRelationshipType', 0, 1, 890),
    ('Primo(a)', 'Cousin', 'staffRelationshipType', 0, 1, 900),
    ('Tio(a)', 'Uncle/Aunt', 'staffRelationshipType', 0, 1, 910),
    ('Nieto(a)', 'Grandchild', 'staffRelationshipType', 0, 1, 920);

    COMMIT TRANSACTION;
    PRINT 'Tipos de parentesco actualizados correctamente.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

