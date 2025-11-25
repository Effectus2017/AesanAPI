-- =============================================
-- Script: Actualizar SPs nuevos para eliminar lógica de compatibilidad
-- Fecha: [Fecha de implementación]
-- Descripción: Elimina código que actualiza IsOwner/IsMonitor de los SPs nuevos
-- ⚠️ ADVERTENCIA: Solo ejecutar DESPUÉS de eliminar campos IsOwner/IsMonitor
-- =============================================

-- NOTA: Este script contiene instrucciones para actualizar cada SP nuevo individualmente.
-- Revisar cada SP y eliminar referencias a IsOwner/IsMonitor manualmente.

-- =============================================
-- SPs a Actualizar:
-- =============================================

-- 1. 102_AssignAgencyToUser
--    - ELIMINAR del INSERT: IsOwner, IsMonitor
--    - ELIMINAR del VALUES: 
--      CASE WHEN @assignmentType = 'AGENCY_OWNER' THEN 1 ELSE 0 END,
--      CASE WHEN @assignmentType LIKE 'NUTRE_%' THEN 1 ELSE 0 END
--    - ELIMINAR del UPDATE:
--      IsOwner = CASE WHEN @assignmentType = 'AGENCY_OWNER' THEN 1 ELSE 0 END,
--      IsMonitor = CASE WHEN @assignmentType LIKE 'NUTRE_%' THEN 1 ELSE 0 END

-- 2. 102_UnassignAgencyToUser
--    - ELIMINAR cualquier referencia a IsOwner/IsMonitor

-- 3. 104_GetUserAssignedAgency
--    - ELIMINAR del SELECT: au.IsOwner, au.IsMonitor

-- 4. 101_GetUserAssignedAgencies
--    - ELIMINAR del SELECT: au.IsOwner, au.IsMonitor

-- 5. 102_UpdateUserMainAgency
--    - ELIMINAR del UPDATE: IsOwner, IsMonitor
--    - ELIMINAR del INSERT: IsOwner, IsMonitor

-- 6. 111_UpdateUser
--    - ELIMINAR del UPDATE: IsOwner, IsMonitor
--    - ELIMINAR del INSERT: IsOwner, IsMonitor

-- 7. 114_GetAgencyById
--    - ELIMINAR del SELECT: au.IsOwner, au.IsMonitor (si los retorna)

-- 8. 113_GetAgencyByIdAndUserId
--    - ELIMINAR del SELECT: au.IsOwner, au.IsMonitor (si los retorna)

-- 9. 118_GetAgencies
--    - ELIMINAR del SELECT: au.IsOwner, au.IsMonitor (si los retorna)
--    - ELIMINAR de subqueries WHERE: IsOwner = 1, IsMonitor = 1

-- 10. 101_GetAesanDashboardMetrics
--     - ELIMINAR de CTEs WHERE: IsOwner = 1, IsMonitor = 1
--     - Reemplazar con: AssignmentType = 'AGENCY_OWNER', AssignmentType LIKE 'NUTRE_%'

-- =============================================
-- Ejemplo de Cambio:
-- =============================================

-- ANTES (con compatibilidad):
/*
INSERT INTO AgencyUsers (
    AssignmentType,
    RoleId,
    IsOwner,      -- ELIMINAR
    IsMonitor,    -- ELIMINAR
    IsActive,
    AssignedBy
)
VALUES (
    @assignmentType,
    @roleId,
    CASE WHEN @assignmentType = 'AGENCY_OWNER' THEN 1 ELSE 0 END,  -- ELIMINAR
    CASE WHEN @assignmentType LIKE 'NUTRE_%' THEN 1 ELSE 0 END,     -- ELIMINAR
    1,
    @assignedBy
);
*/

-- DESPUÉS (sin compatibilidad):
/*
INSERT INTO AgencyUsers (
    AssignmentType,
    RoleId,
    IsActive,
    AssignedBy,
    CreatedAt,
    AssignedDate
)
VALUES (
    @assignmentType,
    @roleId,
    1,
    @assignedBy,
    GETUTCDATE(),
    GETUTCDATE()
);
*/

-- =============================================
-- INSTRUCCIONES:
-- =============================================
-- 1. Abrir cada SP nuevo en un editor
-- 2. Buscar todas las referencias a IsOwner e IsMonitor
-- 3. Eliminar esas líneas
-- 4. Verificar que el SP sigue funcionando correctamente
-- 5. Probar en ambiente de desarrollo antes de producción

PRINT '⚠️ Este script contiene solo instrucciones.';
PRINT '⚠️ Actualizar cada SP nuevo manualmente según las instrucciones arriba.';
PRINT '⚠️ NO ejecutar este script directamente.';
GO

