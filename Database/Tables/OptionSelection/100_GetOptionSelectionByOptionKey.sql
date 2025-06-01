CREATE OR ALTER PROCEDURE [dbo].[100_GetOptionSelectionByOptionKey]
    @optionKey NVARCHAR(255),
    @optionNamesExclude NVARCHAR(255) = null
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear una tabla temporal para almacenar los kyes
    CREATE TABLE #OptionKey
    (
        OptionKey NVARCHAR(255)
    );

    -- Si se proporcionan kyes, insertarlos en la tabla temporal
    IF @optionKey IS NOT NULL
     BEGIN
        INSERT INTO #OptionKey
        SELECT value
        FROM STRING_SPLIT(@optionKey, ',');
    END

    SELECT
        Id,
        Name,
        NameEN,
        OptionKey,
        IsActive,
        DisplayOrder,
        BooleanValue,
        CreatedAt,
        UpdatedAt
    FROM
        OptionSelection
    WHERE 
        IsActive = 1
        AND
        OptionKey IN (SELECT OptionKey
        FROM #OptionKey)
    ORDER BY 
        DisplayOrder ASC;

    -- Si proporcionan nombres excluidos, que no se incluyan en el resultado final
    -- Ejemplo si agregamos "En Proceso" y "Otorgado" en el optionNamesExclude, se excluirán de la consulta
    IF @optionNamesExclude IS NOT NULL
     BEGIN
        DELETE FROM #OptionKey
        WHERE OptionKey IN (SELECT value
        FROM STRING_SPLIT(@optionNamesExclude, ','));
    END

END
