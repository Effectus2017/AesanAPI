CREATE OR ALTER PROCEDURE [100_GetCities]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT,
    @isList BIT = 0
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

        IF @isList = 0
        BEGIN
            SELECT COUNT(*) AS TotalCount
            FROM City;
        END
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

        IF @isList = 0
        BEGIN
            SELECT COUNT(*) AS TotalCount
            FROM City c
            WHERE (@name IS NULL OR c.Name LIKE '%' + @name + '%');
        END
    END
END;
GO