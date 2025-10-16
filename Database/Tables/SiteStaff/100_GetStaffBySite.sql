-- =============================================
-- Stored Procedure: 100_GetStaffBySite
-- Descripción: Obtiene el personal asignado a un sitio
-- Reemplaza: 100_GetStaffBySchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffBySite]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.SiteId,
        ss.StaffId,
        s.FirstName,
        s.LastName,
        s.Email,
        s.Phone,
        ss.AssignmentDate,
        ss.AssignmentTypeId,
        at.Name AS AssignmentTypeName,
        at.NameEN AS AssignmentTypeNameEN,
        ss.IsPrimary,
        ss.StartDate,
        ss.EndDate,
        ss.Comments,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt
    FROM SiteStaff ss
        INNER JOIN Staff s ON ss.StaffId = s.Id
        LEFT JOIN OptionSelection at ON ss.AssignmentTypeId = at.Id
    WHERE ss.SiteId = @siteId AND ss.IsActive = 1
    ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
END;
