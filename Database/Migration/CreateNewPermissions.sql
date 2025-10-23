-- =====================================================
-- Script de Nuevos Permisos - Estructura AESAN
-- Fecha: 2025-01-15
-- Descripción: Crea SOLO los nuevos permisos requeridos para los roles AESAN
-- NOTA: Solo crea permisos que NO existan actualmente en el sistema
-- =====================================================

-- Verificar que la tabla Permission exista
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'Permission')
BEGIN
    RAISERROR('La tabla Permission no existe. Ejecute primero el script de creación de permisos.', 16, 1);
    RETURN;
END

PRINT 'Iniciando creación de nuevos permisos para estructura AESAN...';
PRINT 'NOTA: Solo se crearán permisos que NO existan actualmente en el sistema';

BEGIN TRY
    BEGIN TRANSACTION;

    -- =====================================================
    -- PERMISOS PARA EVALUACIÓN Y VALIDACIÓN
    -- =====================================================
    
    PRINT 'Creando permisos de evaluación y validación...';
    
    -- Permisos de evaluación
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'evaluation.view', 'Ver evaluaciones', 'View evaluations', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'evaluation.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'evaluation.create', 'Crear evaluaciones', 'Create evaluations', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'evaluation.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'evaluation.edit', 'Editar evaluaciones', 'Edit evaluations', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'evaluation.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'evaluation.approve', 'Aprobar evaluaciones', 'Approve evaluations', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'evaluation.approve');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'evaluation.reject', 'Rechazar evaluaciones', 'Reject evaluations', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'evaluation.reject');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'evaluation.delete', 'Eliminar evaluaciones', 'Delete evaluations', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'evaluation.delete');

    -- Permisos de intención de participación
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'intention.view', 'Ver intenciones de participación', 'View participation intentions', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'intention.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'intention.create', 'Crear intenciones de participación', 'Create participation intentions', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'intention.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'intention.edit', 'Editar intenciones de participación', 'Edit participation intentions', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'intention.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'intention.evaluate', 'Evaluar intenciones de participación', 'Evaluate participation intentions', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'intention.evaluate');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'intention.validate', 'Validar intenciones de participación', 'Validate participation intentions', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'intention.validate');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'intention.approve', 'Aprobar intenciones de participación', 'Approve participation intentions', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'intention.approve');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'intention.reject', 'Rechazar intenciones de participación', 'Reject participation intentions', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'intention.reject');

    -- Permisos de validación de sponsor
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'sponsor.data.validate', 'Validar datos del auspiciador', 'Validate sponsor data', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'sponsor.data.validate');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'sponsor.documents.validate', 'Validar documentos del auspiciador', 'Validate sponsor documents', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'sponsor.documents.validate');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'sponsor.forms.validate', 'Validar formularios del auspiciador', 'Validate sponsor forms', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'sponsor.forms.validate');

    -- =====================================================
    -- PERMISOS PARA VISITAS Y ESPECIALISTAS
    -- =====================================================
    
    PRINT 'Creando permisos de visitas y especialistas...';
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.view', 'Ver visitas', 'View visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.create', 'Crear visitas', 'Create visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.edit', 'Editar visitas', 'Edit visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.delete', 'Eliminar visitas', 'Delete visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.delete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.coordinate', 'Coordinar visitas', 'Coordinate visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.coordinate');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.perform', 'Realizar visitas', 'Perform visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.perform');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.document', 'Documentar visitas', 'Document visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.document');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.schedule', 'Programar visitas', 'Schedule visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.schedule');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.reschedule', 'Reprogramar visitas', 'Reschedule visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.reschedule');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.cancel', 'Cancelar visitas', 'Cancel visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.cancel');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.report', 'Generar reportes de visitas', 'Generate visit reports', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.report');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.approve', 'Aprobar visitas', 'Approve visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.approve');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'visit.reject', 'Rechazar visitas', 'Reject visits', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'visit.reject');

    -- =====================================================
    -- PERMISOS PARA ELEGIBILIDAD
    -- =====================================================
    
    PRINT 'Creando permisos de elegibilidad...';
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.view', 'Ver módulo de elegibilidad', 'View eligibility module', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.create', 'Crear elegibilidad', 'Create eligibility', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.edit', 'Editar elegibilidad', 'Edit eligibility', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.delete', 'Eliminar elegibilidad', 'Delete eligibility', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.delete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.approve', 'Aprobar elegibilidad', 'Approve eligibility', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.approve');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.reject', 'Rechazar elegibilidad', 'Reject eligibility', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.reject');

    -- Permisos específicos para funcionarios
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.determine', 'Determinar elegibilidad', 'Determine eligibility', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.determine');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.confirm', 'Confirmar elegibilidad', 'Confirm eligibility', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.confirm');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'eligibility.review', 'Revisión independiente de elegibilidad', 'Independent eligibility review', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'eligibility.review');

    -- =====================================================
    -- PERMISOS PARA CONTRATOS
    -- =====================================================
    
    PRINT 'Creando permisos de contratos...';
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.view', 'Ver contratos', 'View contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.create', 'Crear contratos', 'Create contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.edit', 'Editar contratos', 'Edit contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.delete', 'Eliminar contratos', 'Delete contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.delete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.amend', 'Enmendar contratos', 'Amend contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.amend');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.sign', 'Firmar contratos', 'Sign contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.sign');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.approve', 'Aprobar contratos', 'Approve contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.approve');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'contract.reject', 'Rechazar contratos', 'Reject contracts', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'contract.reject');

    -- =====================================================
    -- PERMISOS PARA CONTABILIDAD
    -- =====================================================
    
    PRINT 'Creando permisos de contabilidad...';
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'accounting.view', 'Ver contabilidad', 'View accounting', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'accounting.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'accounting.create', 'Crear contabilidad', 'Create accounting', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'accounting.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'accounting.edit', 'Editar contabilidad', 'Edit accounting', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'accounting.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'accounting.delete', 'Eliminar contabilidad', 'Delete accounting', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'accounting.delete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'accounting.approve', 'Aprobar contabilidad', 'Approve accounting', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'accounting.approve');

    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'budget.view', 'Ver presupuestos', 'View budgets', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'budget.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'budget.create', 'Crear presupuestos', 'Create budgets', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'budget.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'budget.edit', 'Editar presupuestos', 'Edit budgets', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'budget.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'budget.delete', 'Eliminar presupuestos', 'Delete budgets', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'budget.delete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'budget.approve', 'Aprobar presupuestos', 'Approve budgets', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'budget.approve');

    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'financial.view', 'Ver manejo financiero', 'View financial management', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'financial.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'financial.create', 'Crear manejo financiero', 'Create financial management', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'financial.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'financial.edit', 'Editar manejo financiero', 'Edit financial management', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'financial.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'financial.delete', 'Eliminar manejo financiero', 'Delete financial management', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'financial.delete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'financial.approve', 'Aprobar manejo financiero', 'Approve financial management', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'financial.approve');

    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'payroll.view', 'Ver nómina', 'View payroll', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'payroll.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'payroll.create', 'Crear nómina', 'Create payroll', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'payroll.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'payroll.edit', 'Editar nómina', 'Edit payroll', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'payroll.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'payroll.delete', 'Eliminar nómina', 'Delete payroll', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'payroll.delete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'payroll.approve', 'Aprobar nómina', 'Approve payroll', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'payroll.approve');

    -- =====================================================
    -- PERMISOS PARA SPONSOR/AUSPICIADOR
    -- =====================================================
    
    PRINT 'Creando permisos de sponsor/auspiciador...';
    
    -- Permisos generales del sponsor
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'sponsor.view', 'Ver datos de sponsor', 'View sponsor data', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'sponsor.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'sponsor.edit', 'Editar datos de sponsor', 'Edit sponsor data', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'sponsor.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'sponsor.approve', 'Aprobar datos de sponsor', 'Approve sponsor data', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'sponsor.approve');

    -- =====================================================
    -- PERMISOS PARA FORMULARIOS
    -- =====================================================
    
    PRINT 'Creando permisos de formularios...';
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'form.view', 'Ver formularios', 'View forms', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'form.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'form.create', 'Crear formularios', 'Create forms', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'form.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'form.edit', 'Editar formularios', 'Edit forms', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'form.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'form.complete', 'Completar formularios', 'Complete forms', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'form.complete');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'form.approve', 'Aprobar formularios', 'Approve forms', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'form.approve');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'form.reject', 'Rechazar formularios', 'Reject forms', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'form.reject');

    -- =====================================================
    -- PERMISOS PARA NUTRICIÓN
    -- =====================================================
    
    PRINT 'Creando permisos de nutrición...';
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'nutrition.view', 'Ver módulo de nutrición', 'View nutrition module', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'nutrition.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'nutrition.create', 'Crear módulo de nutrición', 'Create nutrition module', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'nutrition.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'nutrition.edit', 'Editar módulo de nutrición', 'Edit nutrition module', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'nutrition.edit');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'nutrition.approve', 'Aprobar módulo de nutrición', 'Approve nutrition module', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'nutrition.approve');

    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'menu.view', 'Ver menús', 'View menus', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'menu.view');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'menu.create', 'Crear menús', 'Create menus', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'menu.create');
    
    INSERT INTO Permission
    (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
SELECT NEWID(), 'menu.edit', 'Editar menús', 'Edit menus', 1, GETDATE(), GETDATE()
WHERE NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'menu.edit');

    COMMIT TRANSACTION;
    
    PRINT '=====================================================';
    PRINT 'Creación de nuevos permisos completada exitosamente!';
    PRINT '=====================================================';
    
    -- Mostrar resumen de permisos creados
    DECLARE @NewPermissionsCount INT;
    SELECT @NewPermissionsCount = COUNT(*)
FROM Permission
WHERE CreatedAt >= DATEADD(MINUTE, -5, GETDATE());
    
    PRINT 'Total de nuevos permisos creados: ' + CAST(@NewPermissionsCount AS VARCHAR(10));
    
    -- Mostrar permisos por categoría
    PRINT 'Permisos creados por categoría:';
    SELECT
    CASE 
            WHEN ValueKey LIKE 'evaluation.%' OR ValueKey LIKE 'intention.%' OR ValueKey LIKE 'sponsor.data.%' OR ValueKey LIKE 'sponsor.documents.%' OR ValueKey LIKE 'sponsor.forms.%' THEN 'Evaluación y Validación'
            WHEN ValueKey LIKE 'visit.%' THEN 'Visitas'
            WHEN ValueKey LIKE 'eligibility.%' THEN 'Elegibilidad'
            WHEN ValueKey LIKE 'contract.%' THEN 'Contratos'
            WHEN ValueKey LIKE 'accounting.%' OR ValueKey LIKE 'budget.%' OR ValueKey LIKE 'financial.%' OR ValueKey LIKE 'payroll.%' THEN 'Contabilidad'
            WHEN ValueKey LIKE 'sponsor.%' THEN 'Sponsor/Auspiciador'
            WHEN ValueKey LIKE 'form.%' THEN 'Formularios'
            WHEN ValueKey LIKE 'nutrition.%' OR ValueKey LIKE 'menu.%' THEN 'Nutrición'
            ELSE 'Otros'
        END AS Categoria,
    COUNT(*) AS Cantidad
FROM Permission
WHERE CreatedAt >= DATEADD(MINUTE, -5, GETDATE())
GROUP BY 
        CASE 
            WHEN ValueKey LIKE 'evaluation.%' OR ValueKey LIKE 'intention.%' OR ValueKey LIKE 'sponsor.data.%' OR ValueKey LIKE 'sponsor.documents.%' OR ValueKey LIKE 'sponsor.forms.%' THEN 'Evaluación y Validación'
            WHEN ValueKey LIKE 'visit.%' THEN 'Visitas'
            WHEN ValueKey LIKE 'eligibility.%' THEN 'Elegibilidad'
            WHEN ValueKey LIKE 'contract.%' THEN 'Contratos'
            WHEN ValueKey LIKE 'accounting.%' OR ValueKey LIKE 'budget.%' OR ValueKey LIKE 'financial.%' OR ValueKey LIKE 'payroll.%' THEN 'Contabilidad'
            WHEN ValueKey LIKE 'sponsor.%' THEN 'Sponsor/Auspiciador'
            WHEN ValueKey LIKE 'form.%' THEN 'Formularios'
            WHEN ValueKey LIKE 'nutrition.%' OR ValueKey LIKE 'menu.%' THEN 'Nutrición'
            ELSE 'Otros'
        END
ORDER BY Cantidad DESC;

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    
    PRINT '=====================================================';
    PRINT 'ERROR en creación de permisos:';
    PRINT 'Mensaje: ' + @ErrorMessage;
    PRINT 'Número: ' + CAST(@ErrorNumber AS VARCHAR(10));
    PRINT 'Línea: ' + CAST(@ErrorLine AS VARCHAR(10));
    PRINT '=====================================================';
    
    THROW;
END CATCH;

GO