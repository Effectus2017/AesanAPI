-- Versión 101: orden configurable por el cliente (sortByNameKeys, sortByNameENKeys, language).
-- Parámetros opcionales @sortByNameKeys, @sortByNameENKeys, @language para ORDER BY dinámico.
CREATE OR ALTER PROCEDURE [dbo].[101_GetOptionSelectionByOptionKey]
    @optionKey NVARCHAR(MAX),
    @names NVARCHAR(MAX),
    @sortByNameKeys NVARCHAR(MAX) = NULL,
    @sortByNameENKeys NVARCHAR(MAX) = NULL,
    @language NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #OptionKeys
    (
        OptionKey NVARCHAR(255)
    );

    CREATE TABLE #OptionNames
    (
        Name NVARCHAR(255)
    );

    IF @optionKey IS NOT NULL
    BEGIN
        INSERT INTO #OptionKeys
        SELECT LTRIM(RTRIM(value))
        FROM STRING_SPLIT(@optionKey, ',');
    END

    IF @names IS NOT NULL
    BEGIN
        INSERT INTO #OptionNames
        SELECT LTRIM(RTRIM(value))
        FROM STRING_SPLIT(@names, ',');
    END

    CREATE TABLE #SortByNameKeys
    (
        OptionKey NVARCHAR(255)
    );

    CREATE TABLE #SortByNameENKeys
    (
        OptionKey NVARCHAR(255)
    );

    IF @sortByNameKeys IS NOT NULL AND LEN(LTRIM(RTRIM(@sortByNameKeys))) > 0
    BEGIN
        INSERT INTO #SortByNameKeys
        SELECT LTRIM(RTRIM(value))
        FROM STRING_SPLIT(@sortByNameKeys, ',');
    END

    IF @sortByNameENKeys IS NOT NULL AND LEN(LTRIM(RTRIM(@sortByNameENKeys))) > 0
    BEGIN
        INSERT INTO #SortByNameENKeys
        SELECT LTRIM(RTRIM(value))
        FROM STRING_SPLIT(@sortByNameENKeys, ',');
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
        AND (@optionKey IS NULL OR OptionKey IN (SELECT OptionKey FROM #OptionKeys))
        AND (
            @names IS NULL
            OR Name IN (SELECT Name FROM #OptionNames)
            OR OptionKey NOT IN (
                SELECT DISTINCT os.OptionKey
                FROM OptionSelection os
                INNER JOIN #OptionNames n ON os.Name = n.Name
                WHERE os.IsActive = 1
            )
        )
    ORDER BY
        OptionKey,
        CASE
            WHEN OptionKey IN (SELECT OptionKey FROM #SortByNameKeys) OR OptionKey IN (SELECT OptionKey FROM #SortByNameENKeys) THEN
                CASE 
                    WHEN Name = 'N/A' OR NameEN = 'N/A' THEN 0
                    ELSE 1
                END
            ELSE 0
        END,
        CASE
            WHEN OptionKey IN (SELECT OptionKey FROM #SortByNameKeys) AND @language = 'en' THEN NameEN
            WHEN OptionKey IN (SELECT OptionKey FROM #SortByNameKeys) THEN Name
            WHEN OptionKey IN (SELECT OptionKey FROM #SortByNameENKeys) THEN NameEN
            ELSE NULL
        END,
        DisplayOrder ASC;

    DROP TABLE #OptionKeys;
    DROP TABLE #OptionNames;
    DROP TABLE #SortByNameKeys;
    DROP TABLE #SortByNameENKeys;
END;
