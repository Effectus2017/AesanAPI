-- =============================================
-- Stored Procedure: 100_GetStaffTypes
-- =============================================
-- Obtiene todos los tipos de staff con paginación y filtros
-- Parámetros:
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @name: Nombre para filtrar
--   @alls: Si es true, retorna solo lista simple sin paginación

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffTypes]
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @alls = 1
    BEGIN
        -- Retorna lista simple para dropdowns
        SELECT
            Id,
            Name,
            NameEn,
            SortOrder,
            IsActive,
            CreatedAt,
            UpdatedAt
        FROM StaffType
        WHERE IsActive = 1
        ORDER BY SortOrder, Name;
    END
    ELSE
    BEGIN
        -- Retorna lista paginada con filtros
        SELECT
            Id,
            Name,
            NameEn,
            SortOrder,
            IsActive,
            CreatedAt,
            UpdatedAt
        FROM StaffType
        WHERE IsActive = 1
            AND (
                @alls = 1
            OR (@name IS NULL OR
            Name LIKE '%' + @name + '%' OR
            NameEn LIKE '%' + @name + '%')
            )
        ORDER BY SortOrder, Name
        OFFSET @skip ROWS
        FETCH NEXT @take ROWS ONLY;

        -- Total de registros para paginación
        SELECT COUNT(*)
        FROM StaffType
        WHERE IsActive = 1
            AND (
                @alls = 1
            OR (@name IS NULL OR
            Name LIKE '%' + @name + '%' OR
            NameEn LIKE '%' + @name + '%')
            );
    END
END