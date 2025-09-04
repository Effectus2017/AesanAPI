-- Script para limpiar registros duplicados en AgencyProgram
-- Ejecutar antes de agregar el constraint único

-- 1. Identificar duplicados
SELECT
    AgencyId,
    ProgramId,
    COUNT(*) as DuplicateCount
FROM AgencyProgram
GROUP BY AgencyId, ProgramId
HAVING COUNT(*) > 1;

-- 2. Mostrar todos los registros duplicados con sus IDs
WITH
    DuplicateRecords
    AS
    (
        SELECT
            Id,
            AgencyId,
            ProgramId,
            CreatedAt,
            ROW_NUMBER() OVER (PARTITION BY AgencyId, ProgramId ORDER BY CreatedAt ASC) as RowNum
        FROM AgencyProgram
    )
SELECT
    Id,
    AgencyId,
    ProgramId,
    CreatedAt,
    CASE WHEN RowNum = 1 THEN 'KEEP' ELSE 'DELETE' END as Action
FROM DuplicateRecords
WHERE AgencyId IN (
    SELECT AgencyId
FROM AgencyProgram
GROUP BY AgencyId, ProgramId
HAVING COUNT(*) > 1
)
ORDER BY AgencyId, ProgramId, CreatedAt;

-- 3. Eliminar duplicados manteniendo solo el registro más antiguo
WITH
    DuplicateRecords
    AS
    (
        SELECT
            Id,
            AgencyId,
            ProgramId,
            ROW_NUMBER() OVER (PARTITION BY AgencyId, ProgramId ORDER BY CreatedAt ASC) as RowNum
        FROM AgencyProgram
    )
DELETE FROM AgencyProgram
WHERE Id IN (
    SELECT Id
FROM DuplicateRecords
WHERE RowNum > 1
);

-- 4. Verificar que no hay duplicados
SELECT
    AgencyId,
    ProgramId,
    COUNT(*) as RecordCount
FROM AgencyProgram
GROUP BY AgencyId, ProgramId
HAVING COUNT(*) > 1;

-- 5. Agregar el constraint único
ALTER TABLE AgencyProgram 
ADD CONSTRAINT UQ_AgencyProgram_AgencyId_ProgramId 
UNIQUE (AgencyId, ProgramId);
