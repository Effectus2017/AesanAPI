-- Insertar relaciones entre GroupType y Program
-- Basado en las reglas de negocio:
-- PDAM: Comedor, Satélite, Salón de Clases
-- PSAV: Comedor, Consume en Comedor-Grupo Externo, Consume en Comedor-Grupo Interno, Domicilio, N/A, Non-Congregate, Satélite, Servi-Carro, Servi-Expreso/Carro
-- PACNA: Comedor, Salones

-- Obtener IDs de Program
DECLARE @ProgramPDAM INT = (SELECT Id
FROM Program
WHERE Name = 'PDAM');
DECLARE @ProgramPSAV INT = (SELECT Id
FROM Program
WHERE Name = 'PSAV');
DECLARE @ProgramPACNA INT = (SELECT Id
FROM Program
WHERE Name = 'PACNA');

-- Obtener IDs de GroupType para PDAM
DECLARE @GroupTypeComedor INT = (SELECT Id
FROM GroupType
WHERE Name = 'Comedor');
DECLARE @GroupTypeSatelite INT = (SELECT Id
FROM GroupType
WHERE Name = 'Satélite');
DECLARE @GroupTypeSalonClases INT = (SELECT Id
FROM GroupType
WHERE Name = 'Salón de Clases');

-- Obtener IDs de GroupType para PSAV
DECLARE @GroupTypeNA INT = (SELECT Id
FROM GroupType
WHERE Name = 'N/A');
DECLARE @GroupTypeConsumeExterno INT = (SELECT Id
FROM GroupType
WHERE Name = 'Consume en Comedor-Grupo Externo');
DECLARE @GroupTypeConsumeInterno INT = (SELECT Id
FROM GroupType
WHERE Name = 'Consume en Comedor-Grupo Interno');
DECLARE @GroupTypeDomicilio INT = (SELECT Id
FROM GroupType
WHERE Name = 'Domicilio');
DECLARE @GroupTypeNonCongregate INT = (SELECT Id
FROM GroupType
WHERE Name = 'Non-Congregate');
DECLARE @GroupTypeServiCarro INT = (SELECT Id
FROM GroupType
WHERE Name = 'Servi-Carro');
DECLARE @GroupTypeServiExpresoCarro INT = (SELECT Id
FROM GroupType
WHERE Name = 'Servi-Expreso/Carro');

-- Obtener IDs de GroupType para PACNA
DECLARE @GroupTypeSalones INT = (SELECT Id
FROM GroupType
WHERE Name = 'Salones');

-- Insertar relaciones para PDAM
IF @ProgramPDAM IS NOT NULL AND @GroupTypeComedor IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeComedor, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @GroupTypeSatelite IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeSatelite, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @GroupTypeSalonClases IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeSalonClases, @ProgramPDAM);
END

-- Insertar relaciones para PSAV
IF @ProgramPSAV IS NOT NULL AND @GroupTypeComedor IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeComedor, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeConsumeExterno IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeConsumeExterno, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeConsumeInterno IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeConsumeInterno, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeDomicilio, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeNA IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeNA, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeNonCongregate IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeNonCongregate, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeSatelite IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeSatelite, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeServiCarro IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeServiCarro, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @GroupTypeServiExpresoCarro IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeServiExpresoCarro, @ProgramPSAV);
END

-- Insertar relaciones para PACNA
IF @ProgramPACNA IS NOT NULL AND @GroupTypeComedor IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeComedor, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @GroupTypeSalones IS NOT NULL
BEGIN
    INSERT INTO GroupTypeProgram
        (GroupTypeId, ProgramId)
    VALUES
        (@GroupTypeSalones, @ProgramPACNA);
END

