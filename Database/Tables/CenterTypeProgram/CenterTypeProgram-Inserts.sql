-- Insertar relaciones entre CenterType y Program
-- Basado en las reglas de negocio:
-- PDAM: Centro Correccional Juvenil, Centro de Tratamiento Residencial para Salud Mental, Orfanato
-- PACNA: Albergue, Centro de Cuidado Adulto, Centro de Cuidado Diurno

-- Obtener IDs de Program
DECLARE @ProgramPDAM INT = (SELECT Id
FROM Program
WHERE Name = 'PDAM');
DECLARE @ProgramPACNA INT = (SELECT Id
FROM Program
WHERE Name = 'PACNA');

-- Obtener IDs de CenterType para PDAM
DECLARE @CenterTypeOrfanato INT = (SELECT Id
FROM CenterType
WHERE Name = 'Orfanato');
DECLARE @CenterTypeTratamientoResidencial INT = (SELECT Id
FROM CenterType
WHERE Name = 'Centro de tratamiento residencial para salud mental');
DECLARE @CenterTypeCorreccionalJuvenil INT = (SELECT Id
FROM CenterType
WHERE Name = 'Centro Correccional Juvenil');

-- Obtener IDs de CenterType para PACNA
DECLARE @CenterTypeAlbergue INT = (SELECT Id
FROM CenterType
WHERE Name = 'Albergue');
DECLARE @CenterTypeCuidadoAdulto INT = (SELECT Id
FROM CenterType
WHERE Name = 'Centro de Cuidado Adulto');
DECLARE @CenterTypeCuidadoDiurno INT = (SELECT Id
FROM CenterType
WHERE Name = 'Centro de Cuidado Diurno');

-- Insertar relaciones para PDAM
IF @ProgramPDAM IS NOT NULL AND @CenterTypeOrfanato IS NOT NULL
BEGIN
    INSERT INTO CenterTypeProgram
        (CenterTypeId, ProgramId)
    VALUES
        (@CenterTypeOrfanato, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @CenterTypeTratamientoResidencial IS NOT NULL
BEGIN
    INSERT INTO CenterTypeProgram
        (CenterTypeId, ProgramId)
    VALUES
        (@CenterTypeTratamientoResidencial, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @CenterTypeCorreccionalJuvenil IS NOT NULL
BEGIN
    INSERT INTO CenterTypeProgram
        (CenterTypeId, ProgramId)
    VALUES
        (@CenterTypeCorreccionalJuvenil, @ProgramPDAM);
END

-- Insertar relaciones para PACNA
IF @ProgramPACNA IS NOT NULL AND @CenterTypeAlbergue IS NOT NULL
BEGIN
    INSERT INTO CenterTypeProgram
        (CenterTypeId, ProgramId)
    VALUES
        (@CenterTypeAlbergue, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @CenterTypeCuidadoAdulto IS NOT NULL
BEGIN
    INSERT INTO CenterTypeProgram
        (CenterTypeId, ProgramId)
    VALUES
        (@CenterTypeCuidadoAdulto, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @CenterTypeCuidadoDiurno IS NOT NULL
BEGIN
    INSERT INTO CenterTypeProgram
        (CenterTypeId, ProgramId)
    VALUES
        (@CenterTypeCuidadoDiurno, @ProgramPACNA);
END
