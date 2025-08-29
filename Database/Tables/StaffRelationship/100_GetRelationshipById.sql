-- =============================================
-- Stored Procedure: 100_GetRelationshipById
-- =============================================
-- Obtiene una relación de parentesco específica por su ID
-- Incluye información completa de ambos empleados y el tipo de relación

CREATE OR ALTER PROCEDURE [dbo].[100_GetRelationshipById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la relación exista
    IF NOT EXISTS (SELECT 1
    FROM StaffRelationship
    WHERE Id = @id)
        BEGIN
        RAISERROR ('La relación no existe', 16, 1);
        RETURN;
    END

    -- Obtener la relación con información completa
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
        rt.NameEn AS RelationshipTypeEn
    FROM StaffRelationship sr
        INNER JOIN Staff s1 ON sr.StaffId = s1.Id
        INNER JOIN Staff s2 ON sr.RelatedStaffId = s2.Id
        INNER JOIN OptionSelection pos1 ON s1.PositionId = pos1.Id
        INNER JOIN OptionSelection pos2 ON s2.PositionId = pos2.Id
        INNER JOIN StaffType st1 ON s1.StaffTypeId = st1.Id
        INNER JOIN StaffType st2 ON s2.StaffTypeId = st2.Id
        INNER JOIN OptionSelection rt ON sr.RelationshipTypeId = rt.Id
    WHERE sr.Id = @id;
END

EXEC [dbo].[100_GetRelationshipById] 1;