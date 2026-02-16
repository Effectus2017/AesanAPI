-- =============================================
-- Migración: Columna NameEN y descripciones en AspNetRoles
-- Descripción: (1) Añadir columna NameEN (nombre en inglés) si no existe.
--              (2) Actualizar NameEN y Description para todos los roles.
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    RAISERROR('La tabla AspNetRoles no existe.', 16, 1);
    RETURN;
END

-- =====================================================
-- PASO 1: Añadir columna NameEN si no existe (batch aparte para que exista al compilar los UPDATE)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetRoles') AND name = 'NameEN')
BEGIN
    ALTER TABLE AspNetRoles ADD NameEN NVARCHAR(256) NULL;
    PRINT 'Columna NameEN añadida a AspNetRoles.';
END
GO

-- =====================================================
-- PASO 2: Actualizar NameEN y Description por rol
-- =====================================================
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    RAISERROR('La tabla AspNetRoles no existe.', 16, 1);
    RETURN;
END

BEGIN TRY
    BEGIN TRANSACTION;
    -- Super-Administrator
    UPDATE AspNetRoles SET NameEN = N'Super-Administrator', Description = N'Administrador máximo. Acceso total al sistema.', UpdatedAt = GETDATE() WHERE Name = N'Super-Administrator';

    -- Administrator
    UPDATE AspNetRoles SET NameEN = N'Administrator', Description = N'Administrador del portal. Gestión de usuarios y configuración.', UpdatedAt = GETDATE() WHERE Name = N'Administrator';

    -- Agency-Administrator
    UPDATE AspNetRoles SET NameEN = N'Agency-Administrator', Description = N'Administrador de agencia (sponsor). Gestiona los datos de su agencia.', UpdatedAt = GETDATE() WHERE Name = N'Agency-Administrator';

    -- Agency-User
    UPDATE AspNetRoles SET NameEN = N'Agency-User', Description = N'Usuario de agencia (sponsor). Acceso a los datos de su agencia.', UpdatedAt = GETDATE() WHERE Name = N'Agency-User';

    -- Roles NUTRE (lista AESAN)
    UPDATE AspNetRoles SET NameEN = N'Finance', Description = N'Área de finanzas.', UpdatedAt = GETDATE() WHERE Name = N'Finanzas';
    UPDATE AspNetRoles SET NameEN = N'Accountant', Description = N'Contabilidad y presupuesto.', UpdatedAt = GETDATE() WHERE Name = N'Contable';
    UPDATE AspNetRoles SET NameEN = N'Monitoring Coordinator', Description = N'Coordinación de monitoría en terreno.', UpdatedAt = GETDATE() WHERE Name = N'Coordinadora de Monitoría';
    UPDATE AspNetRoles SET NameEN = N'Compliance Officer', Description = N'Oficial de cumplimiento.', UpdatedAt = GETDATE() WHERE Name = N'Oficial de Cumplimiento';
    UPDATE AspNetRoles SET NameEN = N'Specialist', Description = N'Especialista técnico o de programa.', UpdatedAt = GETDATE() WHERE Name = N'Especialista';
    UPDATE AspNetRoles SET NameEN = N'Analyst', Description = N'Análisis de datos y reportes.', UpdatedAt = GETDATE() WHERE Name = N'Analista';
    UPDATE AspNetRoles SET NameEN = N'Coordinator', Description = N'Coordinación general.', UpdatedAt = GETDATE() WHERE Name = N'Coordinadora';
    UPDATE AspNetRoles SET NameEN = N'Evaluator', Description = N'Evaluación de agencias o programas.', UpdatedAt = GETDATE() WHERE Name = N'Evaluadora';
    UPDATE AspNetRoles SET NameEN = N'Nutritionist', Description = N'Área de nutrición.', UpdatedAt = GETDATE() WHERE Name = N'Nutricionista';
    UPDATE AspNetRoles SET NameEN = N'Legal Advisor', Description = N'Asesoría legal.', UpdatedAt = GETDATE() WHERE Name = N'Asesor Legal';

    -- Roles inactivos (por si existen)
    UPDATE AspNetRoles SET NameEN = N'Program Coordinator', Description = N'Empleado de AESAN. Coordinación de programas.', UpdatedAt = GETDATE() WHERE Name = N'Program-Coordinator';
    UPDATE AspNetRoles SET NameEN = N'Director', Description = N'Contabilidad y presupuesto. Dirección.', UpdatedAt = GETDATE() WHERE Name = N'Director';
    UPDATE AspNetRoles SET NameEN = N'Accounting', Description = N'Contabilidad y presupuesto.', UpdatedAt = GETDATE() WHERE Name = N'Accounting';

    -- Roles legacy por si existen (Coordinador, Evaluador, Nutrición, Abogado, etc.)
    UPDATE AspNetRoles SET NameEN = N'Coordinator', Description = N'Coordinador (legacy).', UpdatedAt = GETDATE() WHERE Name = N'Coordinador' AND (NameEN IS NULL OR NameEN = N'');
    UPDATE AspNetRoles SET NameEN = N'Evaluator', Description = N'Evaluador (legacy).', UpdatedAt = GETDATE() WHERE Name = N'Evaluador' AND (NameEN IS NULL OR NameEN = N'');
    UPDATE AspNetRoles SET NameEN = N'Nutrition', Description = N'Área de nutrición (legacy).', UpdatedAt = GETDATE() WHERE Name = N'Nutrición' AND (NameEN IS NULL OR NameEN = N'');
    UPDATE AspNetRoles SET NameEN = N'Attorney', Description = N'Asesoría legal (legacy).', UpdatedAt = GETDATE() WHERE Name = N'Abogado' AND (NameEN IS NULL OR NameEN = N'');

    COMMIT TRANSACTION;
    PRINT 'Migración Mig_AspNetRolesNameENAndDescriptions completada.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT 'Error: ' + @Msg;
    THROW;
END CATCH;
GO
