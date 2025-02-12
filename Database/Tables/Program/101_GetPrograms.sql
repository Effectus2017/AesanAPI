CREATE OR ALTER PROCEDURE [dbo].[101_GetPrograms]
    @take INT,
    @skip INT,
    @names NVARCHAR(MAX),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear una tabla temporal para almacenar los nombres
    CREATE TABLE #ProgramNames (Name NVARCHAR(255));
    
    -- Si se proporcionan nombres, insertarlos en la tabla temporal
    IF @names IS NOT NULL
    BEGIN
        INSERT INTO #ProgramNames
        SELECT value FROM STRING_SPLIT(@names, ',');
    END

    -- Consulta principal
    SELECT Id,
           Name,
           Description
    FROM Program
    WHERE (@alls = 1)
       OR (@names IS NULL)
       OR (Name IN (SELECT Name FROM #ProgramNames))
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Obtener el conteo total
    SELECT COUNT(*) 
    FROM Program
    WHERE (@alls = 1)
       OR (@names IS NULL)
       OR (Name IN (SELECT Name FROM #ProgramNames));

    -- Limpiar
    DROP TABLE #ProgramNames;
END;
GO 