-- =============================================
-- Stored Procedure: 100_GetAllActiveStaffRelationships
-- Versión: 1.1 (sobre 100_GetAllActiveStaffRelationships)
-- Remueve @isList; SP siempre devuelve TotalCount + datos (2 result sets).
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllActiveStaffRelationships]
    @take INT,
    @skip INT,
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- Primero obtener el conteo total
    SELECT COUNT(*) AS TotalCount
    FROM StaffRelationship sr
    WHERE sr.IsActive = 1;

    -- Luego obtener los datos (todos o paginados según @alls)
    SELECT
        sr.Id,
        sr.StaffId,
        sr.RelatedStaffId,
        sr.RelationshipTypeId,
        sr.IsActive,
        sr.Comment,
        sr.CreatedAt,
        sr.UpdatedAt,
        s1.FirstName + ' ' + s1.FatherLastName + ' ' + s1.MotherLastName AS StaffFullName,
        pos1.Name AS StaffPosition,
        st1.Name AS StaffType,
        s1.Email AS StaffEmail,
        s1.IsActive AS StaffIsActive,
        s2.FirstName + ' ' + s2.FatherLastName + ' ' + s2.MotherLastName AS RelatedStaffFullName,
        pos2.Name AS RelatedStaffPosition,
        st2.Name AS RelatedStaffType,
        s2.Email AS RelatedStaffEmail,
        s2.IsActive AS RelatedStaffIsActive,
        rt.Name AS RelationshipType,
        rt.NameEn AS RelationshipTypeEn,
        rt.DisplayOrder AS RelationshipTypeDisplayOrder
    FROM StaffRelationship sr
        INNER JOIN Staff s1 ON sr.StaffId = s1.Id
        INNER JOIN Staff s2 ON sr.RelatedStaffId = s2.Id
        INNER JOIN OptionSelection pos1 ON s1.PositionId = pos1.Id
        INNER JOIN OptionSelection pos2 ON s2.PositionId = pos2.Id
        INNER JOIN StaffType st1 ON s1.StaffTypeId = st1.Id
        INNER JOIN StaffType st2 ON s2.StaffTypeId = st2.Id
        INNER JOIN OptionSelection rt ON sr.RelationshipTypeId = rt.Id
    WHERE sr.IsActive = 1
        AND (@alls = 1 OR (sr.Id > @skip AND sr.Id <= @skip + @take))
    ORDER BY sr.CreatedAt DESC;
END
