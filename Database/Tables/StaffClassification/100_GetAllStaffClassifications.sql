-- =============================================
-- Stored Procedure: 100_GetAllStaffClassifications
-- =============================================
-- Obtiene todas las clasificaciones de staff
-- Parámetros:
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @name: Nombre para filtrar
--   @alls: Si es true, retorna solo lista simple sin paginación
--   @isList: Si es para lista simple (dropdown)

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllStaffClassifications]
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @alls BIT = 0,
    @isList BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @isList = 1 OR @alls = 1
    BEGIN
        -- Retorna lista simple para dropdowns
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
    END
    ELSE
    BEGIN
        -- Retorna lista paginada con filtros
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

        -- Total de registros para paginación
        SELECT COUNT(*)
        FROM StaffClassification sc
        WHERE sc.IsActive = 1
            AND (@name IS NULL OR
            sc.Name LIKE '%' + @name + '%' OR
            sc.NameEn LIKE '%' + @name + '%');
    END
END 