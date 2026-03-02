-- =============================================
-- Migración: Añadir DefaultAgencyAssignmentType a RoleAssignmentCategory
-- Descripción: Permite que el AgencyAssignmentType se resuelva en DB por userId
--              sin lógica en C#. Para AGENCY se usa CanBeOwner en el SP;
--              para NUTRE se usa esta columna (NUTRE_COORDINATOR, NUTRE_EVALUATOR, etc.).
-- =============================================

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('RoleAssignmentCategory')
    AND name = 'DefaultAgencyAssignmentType'
)
BEGIN
    ALTER TABLE RoleAssignmentCategory
    ADD DefaultAgencyAssignmentType VARCHAR(50) NULL;

    PRINT 'Columna DefaultAgencyAssignmentType añadida a RoleAssignmentCategory.';
END
ELSE
BEGIN
    PRINT 'La columna DefaultAgencyAssignmentType ya existe en RoleAssignmentCategory.';
END
GO

-- Rellenar DefaultAgencyAssignmentType para roles NUTRE según nombre del rol
-- (equivalente al mapeo que estaba en C# CalculateAgencyAssignmentTypeFromRole)
-- Se asignan explícitamente por tipo; el resto (Abogado, Funcionario*, Oficial de Cumplimiento, etc.)
-- reciben el default al final solo para no dejar NULL (el SP usa NUTRE_EVALUATOR si es NULL).

-- NUTRE_COORDINATOR: Coordinador, Coordinadora, Coordinadora de Monitoría (+ variantes en inglés)
UPDATE rac
SET rac.DefaultAgencyAssignmentType = 'NUTRE_COORDINATOR',
    rac.UpdatedAt = GETUTCDATE()
FROM RoleAssignmentCategory rac
INNER JOIN AspNetRoles r ON rac.RoleId = r.Id
WHERE rac.AssignmentCategory = 'NUTRE'
  AND (
      r.Name LIKE N'%Coordinador%'
      OR r.Name = N'Coordinadora'
      OR r.Name = N'Coordinadora de Monitoría'
      OR r.Name LIKE N'%Coordinator%'
      OR r.Name = N'Monitoring Coordinator'
      OR r.Name = N'Program Coordinator'
  );

-- NUTRE_ACCOUNTING: Contable, Director Contable, Finanzas (+ variantes en inglés)
UPDATE rac
SET rac.DefaultAgencyAssignmentType = 'NUTRE_ACCOUNTING',
    rac.UpdatedAt = GETUTCDATE()
FROM RoleAssignmentCategory rac
INNER JOIN AspNetRoles r ON rac.RoleId = r.Id
WHERE rac.AssignmentCategory = 'NUTRE'
  AND (
      r.Name LIKE N'%Contable%'
      OR r.Name = N'Finanzas'
      OR r.Name LIKE N'%Accountant%'
      OR r.Name = N'Accounting'
      OR r.Name = N'Director Contable'
  );

-- NUTRE_ADMIN: Administrator/Administrador (si están en NUTRE)
UPDATE rac
SET rac.DefaultAgencyAssignmentType = 'NUTRE_ADMIN',
    rac.UpdatedAt = GETUTCDATE()
FROM RoleAssignmentCategory rac
INNER JOIN AspNetRoles r ON rac.RoleId = r.Id
WHERE rac.AssignmentCategory = 'NUTRE'
  AND (r.Name = N'Administrator' OR r.Name = N'Administrador');

-- NUTRE_EVALUATOR: solo roles de evaluación (Evaluador, Especialista, Nutrición, Analista, etc.)
UPDATE rac
SET rac.DefaultAgencyAssignmentType = 'NUTRE_EVALUATOR',
    rac.UpdatedAt = GETUTCDATE()
FROM RoleAssignmentCategory rac
INNER JOIN AspNetRoles r ON rac.RoleId = r.Id
WHERE rac.AssignmentCategory = 'NUTRE'
  AND (
      r.Name IN (
          N'Evaluador', N'Evaluadora', N'Especialista', N'Nutrición', N'Nutricionista', N'Analista'
      )
      OR r.Name LIKE N'%Evaluador%'
      OR r.Name LIKE N'%Especialista%'
      OR r.Name LIKE N'%Nutrición%'
      OR r.Name LIKE N'%Nutricionista%'
      OR r.Name = N'Evaluator'
      OR r.Name = N'Specialist'
      OR r.Name = N'Nutrition'
      OR r.Name = N'Nutritionist'
      OR r.Name = N'Analyst'
  );

-- Default: cualquier rol NUTRE que aún tenga NULL (Abogado, Asesor Legal, Funcionario*, Oficial de Cumplimiento, etc.)
-- Si necesitáis otro tipo para alguno de estos, añadir un UPDATE específico antes de este bloque.
UPDATE rac
SET rac.DefaultAgencyAssignmentType = 'NUTRE_EVALUATOR',
    rac.UpdatedAt = GETUTCDATE()
FROM RoleAssignmentCategory rac
INNER JOIN AspNetRoles r ON rac.RoleId = r.Id
WHERE rac.AssignmentCategory = 'NUTRE'
  AND (rac.DefaultAgencyAssignmentType IS NULL);

PRINT 'DefaultAgencyAssignmentType actualizado para roles NUTRE.';
GO
