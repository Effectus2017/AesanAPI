CREATE OR ALTER PROCEDURE [dbo].[100_GetOptionSelectionByOptionKey]
    @optionKey NVARCHAR(MAX),
    @names NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear una tabla temporal para almacenar los optionKeys
    CREATE TABLE #OptionKeys
    (
        OptionKey NVARCHAR(255)
    );

    -- Crear una tabla temporal para almacenar los nombres
    CREATE TABLE #OptionNames
    (
        Name NVARCHAR(255)
    );

    -- Si se proporcionan optionKeys, insertarlos en la tabla temporal
    IF @optionKey IS NOT NULL
    BEGIN
        INSERT INTO #OptionKeys
        SELECT value
        FROM STRING_SPLIT(@optionKey, ',');
    END

    -- Si se proporcionan nombres, insertarlos en la tabla temporal
    IF @names IS NOT NULL
    BEGIN
        INSERT INTO #OptionNames
        SELECT value
        FROM STRING_SPLIT(@names, ',');
    END

    SELECT
        Id,
        Name,
        NameEN,
        OptionKey,
        IsActive,
        DisplayOrder,
        BooleanValue,
        IsDefaultValue,
        CreatedAt,
        UpdatedAt
    FROM
        OptionSelection
    WHERE 
        IsActive = 1
        AND (@optionKey IS NULL OR OptionKey IN (SELECT OptionKey
        FROM #OptionKeys))
        AND (
            @names IS NULL
        OR
        Name IN (SELECT Name
        FROM #OptionNames)
        OR
        OptionKey NOT IN (
                SELECT DISTINCT os.OptionKey
        FROM OptionSelection os
            INNER JOIN #OptionNames n ON os.Name = n.Name
        WHERE os.IsActive = 1
            )
        )
    ORDER BY 
        DisplayOrder ASC;

    -- Limpiar
    DROP TABLE #OptionKeys;
    DROP TABLE #OptionNames;
END;

-- Ejemplos de uso:
-- Obtener todas las opciones de status y employeePosition
--EXEC [100_GetOptionSelectionByOptionKey] @optionKey = 'status,employeePosition', @names = NULL;

-- Obtener solo las opciones específicas de employeePosition
--EXEC [100_GetOptionSelectionByOptionKey] @optionKey = 'employeePosition', @names = 'Presidente,Director,Supervisor';

-- Obtener todas las opciones activas/inactivas
--EXEC [100_GetOptionSelectionByOptionKey] @optionKey = 'isActive', @names = 'Active,Inactive';

-- Obtener todas las opciones de isActive Y solo las opciones específicas de employeePosition
--EXEC [100_GetOptionSelectionByOptionKey] @optionKey = 'isActive,employeePosition', @names = 'Presidente,Director,Supervisor';