-- Insertar relaciones entre OrganizationType y Program
-- Basado en las reglas de negocio:
-- PDAM (Id=1): Escuela, Institución Residencial, Satélite (NO Municipios)
-- PSAV (Id=2): Todos (Escuela, Institución Residencial, Municipios, Satélite)
-- PACNA (Id=3): Todos (Escuela, Institución Residencial, Municipios, Satélite)

-- Obtener IDs de Program
DECLARE @ProgramPDAM INT = 1; -- PDAM
DECLARE @ProgramPSAV INT = 2; -- PSAV
DECLARE @ProgramPACNA INT = 3; -- PACNA

-- Obtener IDs de OrganizationType
DECLARE @OrganizationTypeEscuela INT = (SELECT Id FROM OrganizationType WHERE Name = 'Escuela');
DECLARE @OrganizationTypeSatelite INT = (SELECT Id FROM OrganizationType WHERE Name = 'Satélite');
DECLARE @OrganizationTypeInstitucionResidencial INT = (SELECT Id FROM OrganizationType WHERE Name = 'Institución Residencial');
DECLARE @OrganizationTypeMunicipios INT = (SELECT Id FROM OrganizationType WHERE Name = 'Municipios');

-- Insertar relaciones para PDAM (Id=1) - SIN Municipios
IF @ProgramPDAM IS NOT NULL AND @OrganizationTypeEscuela IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeEscuela, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @OrganizationTypeInstitucionResidencial IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeInstitucionResidencial, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @OrganizationTypeSatelite IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeSatelite, @ProgramPDAM);
END

-- Insertar relaciones para PSAV (Id=2) - TODOS
IF @ProgramPSAV IS NOT NULL AND @OrganizationTypeEscuela IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeEscuela, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @OrganizationTypeInstitucionResidencial IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeInstitucionResidencial, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @OrganizationTypeMunicipios IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeMunicipios, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @OrganizationTypeSatelite IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeSatelite, @ProgramPSAV);
END

-- Insertar relaciones para PACNA (Id=3) - TODOS
IF @ProgramPACNA IS NOT NULL AND @OrganizationTypeEscuela IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeEscuela, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @OrganizationTypeInstitucionResidencial IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeInstitucionResidencial, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @OrganizationTypeMunicipios IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeMunicipios, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @OrganizationTypeSatelite IS NOT NULL
BEGIN
    INSERT INTO OrganizationTypeProgram (OrganizationTypeId, ProgramId)
    VALUES (@OrganizationTypeSatelite, @ProgramPACNA);
END

