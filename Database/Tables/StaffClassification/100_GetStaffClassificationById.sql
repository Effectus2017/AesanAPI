-- =============================================
-- Stored Procedure: 100_GetStaffClassificationById
-- =============================================
-- Obtiene una clasificación de staff por su ID

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffClassificationById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sc.Id,
        sc.Name,
        sc.NameEn,
        sc.SortOrder,
        sc.IsActive,
        sc.CreatedAt,
        sc.UpdatedAt
    FROM StaffClassification sc
    WHERE sc.Id = @id
        AND sc.IsActive = 1;
END 