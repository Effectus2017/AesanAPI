-- Procedimiento para obtener todos los programas con paginación y filtros
CREATE OR ALTER PROCEDURE [100_GetAllPrograms]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @onlyActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name,
           Description,
           IsActive,
           CreatedAt,
           UpdatedAt
    FROM Program
    WHERE (@onlyActive = 0 OR IsActive = 1)
      AND (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Retornar el total de registros para la paginación
    SELECT COUNT(*) AS Total
    FROM Program
    WHERE (@onlyActive = 0 OR IsActive = 1)
      AND (@name IS NULL OR Name LIKE '%' + @name + '%');
END;
GO

-- Procedimiento para obtener un programa por su Id
CREATE OR ALTER PROCEDURE [100_GetProgramById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name,
           Description,
           IsActive,
           CreatedAt,
           UpdatedAt
    FROM Program
    WHERE Id = @id;
END;
GO 