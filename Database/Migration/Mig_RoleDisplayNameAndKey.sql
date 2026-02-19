-- =============================================
-- Migración: AspNetRoles - DisplayName, DisplayNameEN y Name como clave única
-- Descripción: (1) Añadir DisplayName y DisplayNameEN.
--              (2) Copiar Name actual a DisplayName y NameEN a DisplayNameEN.
--              (3) Actualizar Name y NormalizedName a claves (lowercase, inglés, guiones bajos).
--              (4) Eliminar columna NameEN (sustituida por DisplayNameEN).
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    RAISERROR('La tabla AspNetRoles no existe.', 16, 1);
    RETURN;
END

-- =====================================================
-- PASO 1: Añadir columnas DisplayName y DisplayNameEN
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetRoles') AND name = 'DisplayName')
BEGIN
    ALTER TABLE AspNetRoles ADD DisplayName NVARCHAR(256) NULL;
    PRINT 'Columna DisplayName añadida a AspNetRoles.';
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetRoles') AND name = 'DisplayNameEN')
BEGIN
    ALTER TABLE AspNetRoles ADD DisplayNameEN NVARCHAR(256) NULL;
    PRINT 'Columna DisplayNameEN añadida a AspNetRoles.';
END
GO

-- =====================================================
-- PASO 2: Copiar valores actuales a DisplayName/DisplayNameEN
-- =====================================================
SET NOCOUNT ON;

UPDATE AspNetRoles
SET DisplayName = Name,
    DisplayNameEN = COALESCE(NULLIF(LTRIM(RTRIM(NameEN)), N''), Name),
    UpdatedAt = GETDATE()
WHERE DisplayName IS NULL OR DisplayNameEN IS NULL;
PRINT 'DisplayName y DisplayNameEN poblados desde Name/NameEN.';
GO

-- =====================================================
-- PASO 3: Actualizar Name y NormalizedName a claves + textos de visualización finales
-- Convención: Name = clave (lowercase, inglés, _); DisplayName = español; DisplayNameEN = inglés
-- =====================================================
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Super-Administrator
    UPDATE AspNetRoles SET Name = N'super_administrator', NormalizedName = N'SUPER_ADMINISTRATOR',
        DisplayName = N'Super-Administrator', DisplayNameEN = N'Super-Administrator', UpdatedAt = GETDATE()
    WHERE Name = N'Super-Administrator';

    -- Administrator
    UPDATE AspNetRoles SET Name = N'administrator', NormalizedName = N'ADMINISTRATOR',
        DisplayName = N'Administrador', DisplayNameEN = N'Administrator', UpdatedAt = GETDATE()
    WHERE Name = N'Administrator';

    -- Agency-Administrator
    UPDATE AspNetRoles SET Name = N'agency_administrator', NormalizedName = N'AGENCY_ADMINISTRATOR',
        DisplayName = N'Administrador de Agencia', DisplayNameEN = N'Agency Administrator', UpdatedAt = GETDATE()
    WHERE Name = N'Agency-Administrator';

    -- Agency-User
    UPDATE AspNetRoles SET Name = N'agency_user', NormalizedName = N'AGENCY_USER',
        DisplayName = N'Usuario de Agencia', DisplayNameEN = N'Agency User', UpdatedAt = GETDATE()
    WHERE Name = N'Agency-User';

    -- Finanzas
    UPDATE AspNetRoles SET Name = N'finance', NormalizedName = N'FINANCE',
        DisplayName = N'Finanzas', DisplayNameEN = N'Finance', UpdatedAt = GETDATE()
    WHERE Name = N'Finanzas';

    -- Contable
    UPDATE AspNetRoles SET Name = N'accountant', NormalizedName = N'ACCOUNTANT',
        DisplayName = N'Contable', DisplayNameEN = N'Accountant', UpdatedAt = GETDATE()
    WHERE Name = N'Contable';

    -- Coordinadora de Monitoría
    UPDATE AspNetRoles SET Name = N'monitoring_coordinator', NormalizedName = N'MONITORING_COORDINATOR',
        DisplayName = N'Coordinadora de Monitoría', DisplayNameEN = N'Monitoring Coordinator', UpdatedAt = GETDATE()
    WHERE Name = N'Coordinadora de Monitoría';

    -- Oficial de Cumplimiento
    UPDATE AspNetRoles SET Name = N'compliance_officer', NormalizedName = N'COMPLIANCE_OFFICER',
        DisplayName = N'Oficial de Cumplimiento', DisplayNameEN = N'Compliance Officer', UpdatedAt = GETDATE()
    WHERE Name = N'Oficial de Cumplimiento';

    -- Especialista
    UPDATE AspNetRoles SET Name = N'specialist', NormalizedName = N'SPECIALIST',
        DisplayName = N'Especialista', DisplayNameEN = N'Specialist', UpdatedAt = GETDATE()
    WHERE Name = N'Especialista';

    -- Analista
    UPDATE AspNetRoles SET Name = N'analyst', NormalizedName = N'ANALYST',
        DisplayName = N'Analista', DisplayNameEN = N'Analyst', UpdatedAt = GETDATE()
    WHERE Name = N'Analista';

    -- Coordinadora (general)
    UPDATE AspNetRoles SET Name = N'coordinator', NormalizedName = N'COORDINATOR',
        DisplayName = N'Coordinadora', DisplayNameEN = N'Coordinator', UpdatedAt = GETDATE()
    WHERE Name = N'Coordinadora';

    -- Evaluadora
    UPDATE AspNetRoles SET Name = N'evaluator', NormalizedName = N'EVALUATOR',
        DisplayName = N'Evaluadora', DisplayNameEN = N'Evaluator', UpdatedAt = GETDATE()
    WHERE Name = N'Evaluadora';

    -- Nutricionista
    UPDATE AspNetRoles SET Name = N'nutritionist', NormalizedName = N'NUTRITIONIST',
        DisplayName = N'Nutricionista', DisplayNameEN = N'Nutritionist', UpdatedAt = GETDATE()
    WHERE Name = N'Nutricionista';

    -- Asesor Legal
    UPDATE AspNetRoles SET Name = N'legal_advisor', NormalizedName = N'LEGAL_ADVISOR',
        DisplayName = N'Asesor Legal', DisplayNameEN = N'Legal Advisor', UpdatedAt = GETDATE()
    WHERE Name = N'Asesor Legal';

    -- Program-Coordinator
    UPDATE AspNetRoles SET Name = N'program_coordinator', NormalizedName = N'PROGRAM_COORDINATOR',
        DisplayName = N'Coordinador de Programa', DisplayNameEN = N'Program Coordinator', UpdatedAt = GETDATE()
    WHERE Name = N'Program-Coordinator';

    -- Director
    UPDATE AspNetRoles SET Name = N'director', NormalizedName = N'DIRECTOR',
        DisplayName = N'Director', DisplayNameEN = N'Director', UpdatedAt = GETDATE()
    WHERE Name = N'Director';

    -- Accounting
    UPDATE AspNetRoles SET Name = N'accounting', NormalizedName = N'ACCOUNTING',
        DisplayName = N'Accounting', DisplayNameEN = N'Accounting', UpdatedAt = GETDATE()
    WHERE Name = N'Accounting';

    -- Legacy: Coordinador
    UPDATE AspNetRoles SET Name = N'coordinator_legacy', NormalizedName = N'COORDINATOR_LEGACY',
        DisplayName = N'Coordinador', DisplayNameEN = N'Coordinator', UpdatedAt = GETDATE()
    WHERE Name = N'Coordinador';

    -- Legacy: Evaluador (solo si no se ha actualizado ya como Evaluadora)
    UPDATE AspNetRoles SET Name = N'evaluator_legacy', NormalizedName = N'EVALUATOR_LEGACY',
        DisplayName = N'Evaluador', DisplayNameEN = N'Evaluator', UpdatedAt = GETDATE()
    WHERE Name = N'Evaluador';

    -- Legacy: Nutrición
    UPDATE AspNetRoles SET Name = N'nutrition', NormalizedName = N'NUTRITION',
        DisplayName = N'Nutrición', DisplayNameEN = N'Nutrition', UpdatedAt = GETDATE()
    WHERE Name = N'Nutrición';

    -- Legacy: Abogado
    UPDATE AspNetRoles SET Name = N'attorney', NormalizedName = N'ATTORNEY',
        DisplayName = N'Abogado', DisplayNameEN = N'Attorney', UpdatedAt = GETDATE()
    WHERE Name = N'Abogado';

    -- =====================================================
    -- PASO 4: Eliminar columna NameEN (ya no necesaria; DisplayNameEN la sustituye)
    -- =====================================================
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetRoles') AND name = 'NameEN')
    BEGIN
        ALTER TABLE AspNetRoles DROP COLUMN NameEN;
        PRINT 'Columna NameEN eliminada de AspNetRoles.';
    END

    COMMIT TRANSACTION;
    PRINT 'Migración Mig_RoleDisplayNameAndKey completada.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT 'Error: ' + @Msg;
    THROW;
END CATCH;
GO
