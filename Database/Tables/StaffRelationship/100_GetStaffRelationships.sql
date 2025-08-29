-- =============================================
-- Stored Procedure: 100_GetStaffRelationships
-- =============================================
-- Obtiene todas las relaciones de parentesco de un empleado específico
-- Incluye información completa de ambos empleados y el tipo de relación

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffRelationships]
    @staffId INT,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el empleado exista
    IF NOT EXISTS (SELECT 1
    FROM Staff
    WHERE Id = @staffId)
        BEGIN
        RAISERROR ('El empleado no existe', 16, 1);
        RETURN;
    END

    -- Obtener las relaciones donde el empleado es el principal
            SELECT
            sr.Id,
            sr.StaffId,
            sr.RelatedStaffId,
            sr.RelationshipTypeId,
            sr.IsActive,
            sr.Comment,
            sr.CreatedAt,
            sr.UpdatedAt,
            -- Información del empleado principal
            s1.FirstName + ' ' + s1.FatherLastName + ' ' + s1.MotherLastName AS StaffFullName,
            pos1.Name AS StaffPosition,
            st1.Name AS StaffType,
            s1.Email AS StaffEmail,
            s1.IsActive AS StaffIsActive,
            -- Información del empleado relacionado
            s2.FirstName + ' ' + s2.FatherLastName + ' ' + s2.MotherLastName AS RelatedStaffFullName,
            pos2.Name AS RelatedStaffPosition,
            st2.Name AS RelatedStaffType,
            s2.Email AS RelatedStaffEmail,
            s2.IsActive AS RelatedStaffIsActive,
            -- Tipo de parentesco
            rt.Name AS RelationshipType,
            rt.NameEn AS RelationshipTypeEn,
            rt.DisplayOrder AS RelationshipTypeDisplayOrder,
            'Direct' AS RelationshipDirection
        FROM StaffRelationship sr
            INNER JOIN Staff s1 ON sr.StaffId = s1.Id
            INNER JOIN Staff s2 ON sr.RelatedStaffId = s2.Id
            INNER JOIN OptionSelection pos1 ON s1.PositionId = pos1.Id
            INNER JOIN OptionSelection pos2 ON s2.PositionId = pos2.Id
            INNER JOIN StaffType st1 ON s1.StaffTypeId = st1.Id
            INNER JOIN StaffType st2 ON s2.StaffTypeId = st2.Id
            INNER JOIN OptionSelection rt ON sr.RelationshipTypeId = rt.Id
        WHERE sr.StaffId = @staffId

    UNION ALL

        -- Obtener las relaciones donde el empleado es el relacionado (relaciones inversas)
        SELECT
            sr.Id,
            sr.RelatedStaffId AS StaffId,
            sr.StaffId AS RelatedStaffId,
            sr.RelationshipTypeId,
            sr.IsActive,
            sr.Comment,
            sr.CreatedAt,
            sr.UpdatedAt,
            -- Información del empleado principal (ahora es el relacionado)
            s2.FirstName + ' ' + s2.FatherLastName + ' ' + s2.MotherLastName AS StaffFullName,
            pos2.Name AS StaffPosition,
            st2.Name AS StaffType,
            s2.Email AS StaffEmail,
            s2.IsActive AS StaffIsActive,
            -- Información del empleado relacionado (ahora es el principal)
            s1.FirstName + ' ' + s1.FatherLastName + ' ' + s1.MotherLastName AS RelatedStaffFullName,
            pos1.Name AS RelatedStaffPosition,
            st1.Name AS RelatedStaffType,
            s1.Email AS RelatedStaffEmail,
            s1.IsActive AS RelatedStaffIsActive,
            -- Tipo de parentesco (puede necesitar inversión)
            CASE 
                WHEN rt.Name LIKE '%Padre%' THEN 'Hijo(a)'
                WHEN rt.Name LIKE '%Madre%' THEN 'Hijo(a)'
                WHEN rt.Name LIKE '%Hijo%' THEN 'Padre/Madre'
                WHEN rt.Name LIKE '%Suegro%' THEN 'Yerno/Nuera'
                WHEN rt.Name LIKE '%Hermano%' THEN 'Hermano(a)'
                WHEN rt.Name LIKE '%Cuñado%' THEN 'Cuñado(a)'
                WHEN rt.Name LIKE '%Esposo%' THEN 'Esposo(a)'
                ELSE rt.Name
            END AS RelationshipType,
            CASE 
                WHEN rt.NameEn LIKE '%Father%' THEN 'Child'
                WHEN rt.NameEn LIKE '%Mother%' THEN 'Child'
                WHEN rt.NameEn LIKE '%Child%' THEN 'Parent'
                WHEN rt.NameEn LIKE '%Father/Mother-in-law%' THEN 'Son/Daughter-in-law'
                WHEN rt.NameEn LIKE '%Sibling%' THEN 'Sibling'
                WHEN rt.NameEn LIKE '%Brother/Sister-in-law%' THEN 'Brother/Sister-in-law'
                WHEN rt.NameEn LIKE '%Spouse%' THEN 'Spouse'
                ELSE rt.NameEn
            END AS RelationshipTypeEn,
            rt.DisplayOrder AS RelationshipTypeDisplayOrder,
            'Inverse' AS RelationshipDirection
        FROM StaffRelationship sr
            INNER JOIN Staff s1 ON sr.StaffId = s1.Id
            INNER JOIN Staff s2 ON sr.RelatedStaffId = s2.Id
            INNER JOIN OptionSelection pos1 ON s1.PositionId = pos1.Id
            INNER JOIN OptionSelection pos2 ON s2.PositionId = pos2.Id
            INNER JOIN StaffType st1 ON s1.StaffTypeId = st1.Id
            INNER JOIN StaffType st2 ON s2.StaffTypeId = st2.Id
            INNER JOIN OptionSelection rt ON sr.RelationshipTypeId = rt.Id
        WHERE sr.RelatedStaffId = @staffId

    ORDER BY RelationshipDirection, RelationshipTypeDisplayOrder, RelatedStaffFullName;
END
EXEC [dbo].[100_GetStaffRelationships] 1;
