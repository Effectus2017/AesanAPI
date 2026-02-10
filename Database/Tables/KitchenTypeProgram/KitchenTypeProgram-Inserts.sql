-- Insertar relaciones entre KitchenType y Program
-- PDAM (Id=1): N/A, (CC), (CCC), (PSS), (CGA)
-- PSAV (Id=2): N/A, (CC), (CCC), (PSS)
-- PACNA (Id=3): N/A, (CC), (CCC), (PSS)

DECLARE @ProgramPDAM INT = 1;
DECLARE @ProgramPSAV INT = 2;
DECLARE @ProgramPACNA INT = 3;

DECLARE @KitchenTypeNA INT = (SELECT Id FROM KitchenType WHERE Name = N'N/A');
DECLARE @KitchenTypeCC INT = (SELECT Id FROM KitchenType WHERE Name = N'(CC) Cocina Central-Solo Satélites');
DECLARE @KitchenTypeCCC INT = (SELECT Id FROM KitchenType WHERE Name = N'(CCC) Cocina Central Combinada- Grupos en Comedor y Satélites');
DECLARE @KitchenTypePSS INT = (SELECT Id FROM KitchenType WHERE Name = N'(PSS) Preparadas y Servidas en el Sitio- Grupos solo en Comedor');
DECLARE @KitchenTypeCGA INT = (SELECT Id FROM KitchenType WHERE Name = N'(CGA) Compañía de Gestión de Alimentos');

-- PDAM: N/A, CC, CCC, PSS, CGA
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeNA IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeNA, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeCC IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCC, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeCCC IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCCC, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypePSS IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypePSS, @ProgramPDAM);
IF @ProgramPDAM IS NOT NULL AND @KitchenTypeCGA IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCGA, @ProgramPDAM);

-- PSAV: N/A, CC, CCC, PSS
IF @ProgramPSAV IS NOT NULL AND @KitchenTypeNA IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeNA, @ProgramPSAV);
IF @ProgramPSAV IS NOT NULL AND @KitchenTypeCC IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCC, @ProgramPSAV);
IF @ProgramPSAV IS NOT NULL AND @KitchenTypeCCC IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCCC, @ProgramPSAV);
IF @ProgramPSAV IS NOT NULL AND @KitchenTypePSS IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypePSS, @ProgramPSAV);

-- PACNA: N/A, CC, CCC, PSS
IF @ProgramPACNA IS NOT NULL AND @KitchenTypeNA IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeNA, @ProgramPACNA);
IF @ProgramPACNA IS NOT NULL AND @KitchenTypeCC IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCC, @ProgramPACNA);
IF @ProgramPACNA IS NOT NULL AND @KitchenTypeCCC IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypeCCC, @ProgramPACNA);
IF @ProgramPACNA IS NOT NULL AND @KitchenTypePSS IS NOT NULL
    INSERT INTO KitchenTypeProgram (KitchenTypeId, ProgramId) VALUES (@KitchenTypePSS, @ProgramPACNA);
