-- =============================================
-- Stored Procedure: 100_GetAllStaffClassifications
-- Versión: 1.1 (sobre 100_GetAllStaffClassifications)
-- Remueve @isList; SP devuelve datos + TotalCount (2 result sets). Si @alls=1 solo datos sin paginar.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllStaffClassifications]
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @alls = 1
    BEGIN
        -- Lista simple (todos), luego count para contrato uniforme
        SELECT
            sc.Id,
            sc.Name,
            sc.NameEn,
            sc.SortOrder,
            sc.IsActive,
            sc.CreatedAt,
            sc.UpdatedAt
        FROM StaffClassification sc
        WHERE sc.IsActive = 1
        ORDER BY sc.SortOrder, sc.Name;

        SELECT COUNT(*)
        FROM StaffClassification sc
        WHERE sc.IsActive = 1;
    END
    ELSE
    BEGIN
        -- Lista paginada con filtros
        SELECT
            sc.Id,
            sc.Name,
            sc.NameEn,
            sc.SortOrder,
            sc.IsActive,
            sc.CreatedAt,
            sc.UpdatedAt
        FROM StaffClassification sc
        WHERE sc.IsActive = 1
            AND (@name IS NULL OR
            sc.Name LIKE '%' + @name + '%' OR
            sc.NameEn LIKE '%' + @name + '%')
        ORDER BY sc.SortOrder, sc.Name
        OFFSET @skip ROWS
        FETCH NEXT @take ROWS ONLY;

        SELECT COUNT(*)
        FROM StaffClassification sc
        WHERE sc.IsActive = 1
            AND (@name IS NULL OR
            sc.Name LIKE '%' + @name + '%' OR
            sc.NameEn LIKE '%' + @name + '%');
    END
END
