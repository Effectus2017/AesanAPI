-- =============================================
-- Migración: Crear tabla RoleAssignmentCategory
-- Descripción: Tabla de mapeo para categorizar roles (AGENCY, NUTRE)
--              y validar qué AgencyAssignmentType es válido para cada rol.
--              NO se usa para determinar acceso a agencias.
-- =============================================

-- Verificar si la tabla ya existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleAssignmentCategory')
BEGIN
    CREATE TABLE RoleAssignmentCategory (
        RoleId NVARCHAR(450) NOT NULL PRIMARY KEY,
        AssignmentCategory VARCHAR(50) NOT NULL, -- 'AGENCY' o 'NUTRE'
        CanBeOwner BIT NOT NULL DEFAULT 0, -- Si el rol puede ser AGENCY_OWNER
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME NULL,
        CONSTRAINT FK_RoleAssignmentCategory_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
    );

    -- Crear índice para AssignmentCategory
    CREATE INDEX IX_RoleAssignmentCategory_AssignmentCategory ON RoleAssignmentCategory (AssignmentCategory);

    PRINT 'Tabla RoleAssignmentCategory creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla RoleAssignmentCategory ya existe.';
END
GO

-- Insertar mapeos para roles existentes
-- IMPORTANTE: Los nombres de roles pueden variar (ej: 'Super-Administrator')
-- Por eso usamos LIKE para buscar variaciones

PRINT 'Insertando mapeos de roles...';

-- Roles AGENCY (roles de agencias cliente)
-- Estos roles pueden tener AgencyAssignmentType = 'AGENCY_OWNER' o 'AGENCY_STAFF'

-- Agency-Administrator / Sponsor-Administrador
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Agency-Administrator' OR Name = 'Sponsor-Administrador' OR Name LIKE '%Sponsor%Administrador%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id, Name
        FROM AspNetRoles
        WHERE (Name = 'Agency-Administrator' OR Name = 'Sponsor-Administrador' OR Name LIKE '%Sponsor%Administrador%')
            AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'AGENCY', 1, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'AGENCY', CanBeOwner = 1, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Agency-Administrator/Sponsor-Administrador insertado';
END

-- Agency-User / Sponsor-User
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Agency-User' OR Name = 'Sponsor-User' OR Name LIKE '%Sponsor%User%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id, Name
        FROM AspNetRoles
        WHERE (Name = 'Agency-User' OR Name = 'Sponsor-User' OR Name LIKE '%Sponsor%User%')
            AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'AGENCY', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'AGENCY', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Agency-User/Sponsor-User insertado';
END

-- Roles NUTRE (roles internos de NUTRE)
-- Estos roles pueden tener AgencyAssignmentType = 'NUTRE_COORDINATOR', 'NUTRE_EVALUATOR', etc.
-- Nota: El rol Monitor fue eliminado; no se incluye en RoleAssignmentCategory.

-- Coordinador
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Coordinador' OR Name LIKE '%Coordinador%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Coordinador' OR Name LIKE '%Coordinador%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Coordinador insertado';
END

-- Evaluador
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Evaluador' OR Name LIKE '%Evaluador%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Evaluador' OR Name LIKE '%Evaluador%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Evaluador insertado';
END

-- Especialista
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Especialista' OR Name LIKE '%Especialista%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Especialista' OR Name LIKE '%Especialista%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Especialista insertado';
END

-- Nutrición
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Nutrición' OR Name LIKE '%Nutrición%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Nutrición' OR Name LIKE '%Nutrición%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Nutrición insertado';
END

-- Contable
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Contable' OR Name LIKE '%Contable%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Contable' OR Name LIKE '%Contable%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Contable insertado';
END

-- Director Contable
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Director Contable' OR Name LIKE '%Director%Contable%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Director Contable' OR Name LIKE '%Director%Contable%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Director Contable insertado';
END

-- Abogado
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Abogado' OR Name LIKE '%Abogado%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Abogado' OR Name LIKE '%Abogado%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Abogado insertado';
END

-- Funcionario Determinante
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Funcionario Determinante' OR Name LIKE '%Funcionario%Determinante%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Funcionario Determinante' OR Name LIKE '%Funcionario%Determinante%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Funcionario Determinante insertado';
END

-- Funcionario Confirmante
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Funcionario Confirmante' OR Name LIKE '%Funcionario%Confirmante%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Funcionario Confirmante' OR Name LIKE '%Funcionario%Confirmante%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Funcionario Confirmante insertado';
END

-- Funcionario Revisión Independiente
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE (Name = 'Funcionario Revisión Independiente' OR Name LIKE '%Funcionario%Revisión%Independiente%') AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (
        SELECT TOP 1 Id
        FROM AspNetRoles
        WHERE (Name = 'Funcionario Revisión Independiente' OR Name LIKE '%Funcionario%Revisión%Independiente%') AND IsActive = 1
    ) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN
        INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt)
        VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN
        UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Funcionario Revisión Independiente insertado';
END

-- Roles NUTRE (lista AESAN): Finanzas, Coordinadora de Monitoría, Oficial de Cumplimiento, Analista, Coordinadora, Evaluadora, Nutricionista, Asesor Legal
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Finanzas' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Finanzas' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Finanzas insertado';
END
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Coordinadora de Monitoría' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Coordinadora de Monitoría' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Coordinadora de Monitoría insertado';
END
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Oficial de Cumplimiento' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Oficial de Cumplimiento' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Oficial de Cumplimiento insertado';
END
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Analista' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Analista' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Analista insertado';
END
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Coordinadora' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Coordinadora' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Coordinadora insertado';
END
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Evaluadora' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Evaluadora' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Evaluadora insertado';
END
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Nutricionista' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Nutricionista' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Nutricionista insertado';
END
IF EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = N'Asesor Legal' AND IsActive = 1)
BEGIN
    MERGE RoleAssignmentCategory AS target
    USING (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = N'Asesor Legal' AND IsActive = 1) AS source ON target.RoleId = source.Id
    WHEN NOT MATCHED THEN INSERT (RoleId, AssignmentCategory, CanBeOwner, CreatedAt) VALUES (source.Id, 'NUTRE', 0, GETUTCDATE())
    WHEN MATCHED THEN UPDATE SET AssignmentCategory = 'NUTRE', CanBeOwner = 0, UpdatedAt = GETUTCDATE();
    PRINT '  - Mapeo para Asesor Legal insertado';
END

-- NOTA: Super-Administrator y Administrator NO se insertan aquí porque:
-- 1. Ven TODAS las agencias (sin filtro AgencyUsers)
-- 2. No necesitan validación de AgencyAssignmentType
-- 3. No se asignan a agencias específicas mediante AgencyUsers

PRINT 'Mapeos de roles insertados exitosamente.';
PRINT '';

-- Verificación: Mostrar roles sin mapeo (alias de salida en lowercase según convención)
PRINT 'Verificando roles sin mapeo...';
SELECT
    id = r.Id,
    rolename = r.Name,
    estado = 'Sin mapeo en RoleAssignmentCategory'
FROM AspNetRoles r
WHERE r.IsActive = 1
    AND r.Name NOT IN ('Super-Administrator', 'Administrator', 'Administrador')
    AND NOT EXISTS (
        SELECT 1
        FROM RoleAssignmentCategory rac
        WHERE rac.RoleId = r.Id
    );

PRINT '';
PRINT 'Script completado.';
GO
