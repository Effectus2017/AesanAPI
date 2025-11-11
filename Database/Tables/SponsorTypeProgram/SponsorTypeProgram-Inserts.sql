-- Insertar relaciones entre SponsorType y Program
-- Basado en las reglas de negocio:
-- PDAM (Id=1): Gobierno, Privado
-- PACNA (Id=3): Privado, Público Estatal, Público Federal

-- Obtener IDs de Program
DECLARE @ProgramPDAM INT = 1; -- PDAM
DECLARE @ProgramPACNA INT = 3; -- PACNA

-- Obtener IDs de SponsorType
DECLARE @SponsorTypeGobierno INT = (SELECT Id FROM SponsorType WHERE Name = 'Gobierno');
DECLARE @SponsorTypePrivado INT = (SELECT Id FROM SponsorType WHERE Name = 'Privado');
DECLARE @SponsorTypePublicoEstatal INT = (SELECT Id FROM SponsorType WHERE Name = 'Público Estatal');
DECLARE @SponsorTypePublicoFederal INT = (SELECT Id FROM SponsorType WHERE Name = 'Público Federal');

-- Insertar relaciones para PDAM (Id=1)
IF @ProgramPDAM IS NOT NULL AND @SponsorTypeGobierno IS NOT NULL
BEGIN
    INSERT INTO SponsorTypeProgram (SponsorTypeId, ProgramId)
    VALUES (@SponsorTypeGobierno, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @SponsorTypePrivado IS NOT NULL
BEGIN
    INSERT INTO SponsorTypeProgram (SponsorTypeId, ProgramId)
    VALUES (@SponsorTypePrivado, @ProgramPDAM);
END

-- Insertar relaciones para PACNA (Id=3)
IF @ProgramPACNA IS NOT NULL AND @SponsorTypePrivado IS NOT NULL
BEGIN
    INSERT INTO SponsorTypeProgram (SponsorTypeId, ProgramId)
    VALUES (@SponsorTypePrivado, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @SponsorTypePublicoEstatal IS NOT NULL
BEGIN
    INSERT INTO SponsorTypeProgram (SponsorTypeId, ProgramId)
    VALUES (@SponsorTypePublicoEstatal, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @SponsorTypePublicoFederal IS NOT NULL
BEGIN
    INSERT INTO SponsorTypeProgram (SponsorTypeId, ProgramId)
    VALUES (@SponsorTypePublicoFederal, @ProgramPACNA);
END

