-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 100_DeleteSiteChildGroupsBySiteId
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSchoolChildGroupsBySchoolId]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la escuela existe
    IF NOT EXISTS (SELECT 1
    FROM School
    WHERE Id = @schoolId)
    BEGIN
        RAISERROR('La escuela especificada no existe.', 16, 1);
        RETURN;
    END

    -- Eliminar todos los grupos de niños de la escuela
    DELETE FROM SchoolChildGroup WHERE SchoolId = @schoolId;
END;
