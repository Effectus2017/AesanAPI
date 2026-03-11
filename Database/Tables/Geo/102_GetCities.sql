-- =============================================
-- Stored Procedure: 100_GetCities
-- Versión: 1.1 (sobre 100_GetCities)
-- Remueve @isList; SP siempre devuelve datos + TotalCount (2 result sets).
-- =============================================

CREATE OR ALTER PROCEDURE [100_GetCities]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF @alls = 1
    BEGIN
        -- Trae toda la tabla, sin filtros ni paginación
        SELECT c.Id,
            c.Name,
            c.IsActive,
            c.CreatedAt,
            c.UpdatedAt
        FROM City c
        ORDER BY c.Name;

        SELECT COUNT(*) AS TotalCount
        FROM City;
    END
    ELSE
    BEGIN
        -- Aplica filtro y paginación
        SELECT c.Id,
            c.Name,
            c.IsActive,
            c.CreatedAt,
            c.UpdatedAt
        FROM City c
        WHERE (@name IS NULL OR c.Name LIKE '%' + @name + '%')
        ORDER BY c.Name
        OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

        SELECT COUNT(*) AS TotalCount
        FROM City c
        WHERE (@name IS NULL OR c.Name LIKE '%' + @name + '%');
    END
END;
GO
