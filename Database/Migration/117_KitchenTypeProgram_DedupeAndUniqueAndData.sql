-- ============================================
-- Migración: Eliminar duplicados en KitchenTypeProgram, añadir UNIQUE y rellenar datos
-- PDAM: N/A, CC, CCC, PSS, CGA. PSAV/PACNA: N/A, CC, CCC, PSS.
-- ============================================

-- 1. Eliminar duplicados: dejar una sola fila por (KitchenTypeId, ProgramId)
;WITH Ranked AS (
    SELECT Id,
        ROW_NUMBER() OVER (PARTITION BY KitchenTypeId, ProgramId ORDER BY Id) AS rn
    FROM KitchenTypeProgram
)
DELETE FROM KitchenTypeProgram
WHERE Id IN (SELECT Id FROM Ranked WHERE rn > 1);

-- 2. Crear índice UNIQUE si no existe (tabla pudo crearse en 116 sin UNIQUE)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.KitchenTypeProgram')
        AND name = N'UX_KitchenTypeProgram_KitchenTypeId_ProgramId'
)
BEGIN
    CREATE UNIQUE INDEX [UX_KitchenTypeProgram_KitchenTypeId_ProgramId]
    ON KitchenTypeProgram (KitchenTypeId, ProgramId);
END

-- 3. Rellenar datos faltantes (identificación tolerante: LIKE para CC, CCC, PSS)
DECLARE @ProgramPDAM INT = 1;
DECLARE @ProgramPSAV INT = 2;
DECLARE @ProgramPACNA INT = 3;

DECLARE @KitchenTypeNA INT = (SELECT Id FROM KitchenType WHERE Name = N'N/A');
DECLARE @KitchenTypeCC INT = (SELECT TOP 1 Id FROM KitchenType WHERE Name LIKE N'(CC)%' ORDER BY Id);
DECLARE @KitchenTypeCCC INT = (SELECT TOP 1 Id FROM KitchenType WHERE Name LIKE N'(CCC)%' ORDER BY Id);
DECLARE @KitchenTypePSS INT = (SELECT TOP 1 Id FROM KitchenType WHERE Name LIKE N'(PSS)%' ORDER BY Id);
DECLARE @KitchenTypeCGA INT = (SELECT Id FROM KitchenType WHERE Name = N'(CGA) Compañía de Gestión de Alimentos');

-- PDAM: N/A, CC, CCC, PSS, CGA
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeNA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeNA AND ProgramId = @ProgramPDAM)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeNA, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeCC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeCC AND ProgramId = @ProgramPDAM)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCC, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeCCC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeCCC AND ProgramId = @ProgramPDAM)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCCC, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypePSS IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypePSS AND ProgramId = @ProgramPDAM)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypePSS, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeCGA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeCGA AND ProgramId = @ProgramPDAM)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCGA, @ProgramPDAM);

-- PSAV: N/A, CC, CCC, PSS
IF @ProgramPSAV IS NOT NULL AND @KitchenTypeNA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeNA AND ProgramId = @ProgramPSAV)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeNA, @ProgramPSAV);
IF @ProgramPSAV IS NOT NULL AND @KitchenTypeCC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeCC AND ProgramId = @ProgramPSAV)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCC, @ProgramPSAV);
IF @ProgramPSAV IS NOT NULL AND @KitchenTypeCCC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeCCC AND ProgramId = @ProgramPSAV)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCCC, @ProgramPSAV);
IF @ProgramPSAV IS NOT NULL AND @KitchenTypePSS IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypePSS AND ProgramId = @ProgramPSAV)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypePSS, @ProgramPSAV);

-- PACNA: N/A, CC, CCC, PSS
IF @ProgramPACNA IS NOT NULL AND @KitchenTypeNA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeNA AND ProgramId = @ProgramPACNA)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeNA, @ProgramPACNA);
IF @ProgramPACNA IS NOT NULL AND @KitchenTypeCC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeCC AND ProgramId = @ProgramPACNA)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCC, @ProgramPACNA);
IF @ProgramPACNA IS NOT NULL AND @KitchenTypeCCC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypeCCC AND ProgramId = @ProgramPACNA)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCCC, @ProgramPACNA);
IF @ProgramPACNA IS NOT NULL AND @KitchenTypePSS IS NOT NULL AND NOT EXISTS (SELECT 1 FROM KitchenTypeProgram WHERE KitchenTypeId = @KitchenTypePSS AND ProgramId = @ProgramPACNA)
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypePSS, @ProgramPACNA);
