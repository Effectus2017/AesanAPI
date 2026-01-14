-- =============================================
-- Script: Agregar campos nuevos a AgencyUsers
-- Fecha: 2025-01-XX
-- Descripción: Agrega el campo AgencyAssignmentType a la tabla AgencyUsers.
--              NO agrega RoleId porque el rol se obtiene mediante JOIN con AspNetUserRoles.
--              Mantiene IsOwner e IsMonitor temporalmente para compatibilidad.
-- =============================================

PRINT 'Agregando campos nuevos a AgencyUsers...';

-- Agregar campo AgencyAssignmentType si no existe
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('AgencyUsers') 
    AND name = 'AgencyAssignmentType'
)
BEGIN
    ALTER TABLE AgencyUsers
    ADD AgencyAssignmentType VARCHAR(50) NULL;

    PRINT '  - Campo AgencyAssignmentType agregado.';
END
ELSE
BEGIN
    PRINT '  - Campo AgencyAssignmentType ya existe.';
END
GO

-- Crear índice para AgencyAssignmentType
IF NOT EXISTS (
    SELECT * 
    FROM sys.indexes 
    WHERE name = 'IX_AgencyUsers_AgencyAssignmentType' 
    AND object_id = OBJECT_ID('AgencyUsers')
)
BEGIN
    CREATE INDEX IX_AgencyUsers_AgencyAssignmentType 
    ON AgencyUsers (AgencyAssignmentType);
    
    PRINT '  - Índice IX_AgencyUsers_AgencyAssignmentType creado.';
END
ELSE
BEGIN
    PRINT '  - Índice IX_AgencyUsers_AgencyAssignmentType ya existe.';
END
GO

-- Crear índice compuesto para búsquedas frecuentes
IF NOT EXISTS (
    SELECT * 
    FROM sys.indexes 
    WHERE name = 'IX_AgencyUsers_AgencyAssignmentType_AgencyId' 
    AND object_id = OBJECT_ID('AgencyUsers')
)
BEGIN
    CREATE INDEX IX_AgencyUsers_AgencyAssignmentType_AgencyId 
    ON AgencyUsers (AgencyAssignmentType, AgencyId)
    INCLUDE (UserId, IsActive);
    
    PRINT '  - Índice compuesto IX_AgencyUsers_AgencyAssignmentType_AgencyId creado.';
END
ELSE
BEGIN
    PRINT '  - Índice compuesto IX_AgencyUsers_AgencyAssignmentType_AgencyId ya existe.';
END
GO

-- Verificar que IsOwner e IsMonitor aún existen (no deben eliminarse en esta fase)
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('AgencyUsers') 
    AND name = 'IsOwner'
) AND EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('AgencyUsers') 
    AND name = 'IsMonitor'
)
BEGIN
    PRINT '  - Campos IsOwner e IsMonitor mantenidos para compatibilidad.';
END
ELSE
BEGIN
    PRINT '  ⚠️ ADVERTENCIA: Campos IsOwner o IsMonitor no encontrados.';
END
GO

PRINT '';
PRINT 'Script completado.';
PRINT 'NOTA: Los campos IsOwner e IsMonitor se mantienen temporalmente para compatibilidad.';
PRINT '      Se eliminarán en la Fase 5 después de verificar que todo funciona correctamente.';
GO
