-- =============================================
-- Stored Procedure: 100_GetAgencyAssignmentTypeByUserId
-- Descripción: Devuelve el AgencyAssignmentType correspondiente al usuario
--              según su rol y RoleAssignmentCategory (lógica antes en C#).
--              Usado por CalculateAgencyAssignmentTypeFromRole en el repositorio.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyAssignmentTypeByUserId]
    @userid NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @assignmentcategory VARCHAR(50);
    DECLARE @canbeowner BIT;
    DECLARE @defaulttype VARCHAR(50);
    DECLARE @result VARCHAR(50);

    -- Obtener categoría y datos del rol del usuario desde RoleAssignmentCategory
    SELECT TOP 1
        @assignmentcategory = rac.AssignmentCategory,
        @canbeowner = rac.CanBeOwner,
        @defaulttype = rac.DefaultAgencyAssignmentType
    FROM AspNetUserRoles ur
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    INNER JOIN RoleAssignmentCategory rac ON rac.RoleId = ur.RoleId
    WHERE ur.UserId = @userid;

    IF @assignmentcategory IS NULL
    BEGIN
        -- Usuario sin rol o rol no configurado en RoleAssignmentCategory (ej. Super-Administrator)
        SELECT agencyassignmenttype = CAST(NULL AS VARCHAR(50));
        RETURN;
    END

    IF @assignmentcategory = 'AGENCY'
    BEGIN
        SET @result = CASE WHEN @canbeowner = 1 THEN 'AGENCY_OWNER' ELSE 'AGENCY_STAFF' END;
    END
    ELSE IF @assignmentcategory = 'NUTRE'
    BEGIN
        SET @result = ISNULL(@defaulttype, 'NUTRE_EVALUATOR');
    END
    ELSE
    BEGIN
        SET @result = NULL;
    END

    SELECT agencyassignmenttype = @result;
END
GO
