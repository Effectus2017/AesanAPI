-- =============================================
-- Script: 120_CleanupEmployeeTables
-- =============================================
-- Limpia las tablas y procedimientos de Employee después de la migración
-- ADVERTENCIA: Este script elimina permanentemente los datos de Employee
-- Solo ejecutar después de verificar que la migración fue exitosa

-- Paso 1: Verificar que la migración fue exitosa
IF NOT EXISTS (SELECT *
FROM Staff
WHERE IsActive = 1)
BEGIN
    PRINT 'Error: No hay datos en la tabla Staff. Verifique la migración antes de continuar.';
    RETURN;
END

-- Paso 2: Mostrar estadísticas antes de la limpieza
PRINT '=== ESTADÍSTICAS ANTES DE LA LIMPIEZA ===';
    SELECT
        'Staff' AS Tabla,
        COUNT(*) AS Registros
    FROM Staff
    WHERE IsActive = 1
UNION ALL
    SELECT
        'Employee' AS Tabla,
        COUNT(*) AS Registros
    FROM Employee
    WHERE IsActive = 1;

-- Paso 3: Confirmar la limpieza
PRINT 'ADVERTENCIA: Este script eliminará permanentemente:';
PRINT '- Tabla Employee';
PRINT '- Procedimientos almacenados de Employee';
PRINT '- Índices de Employee';
PRINT '';
PRINT '¿Está seguro de que desea continuar? (Comente las siguientes líneas si no está seguro)';

-- Paso 4: Eliminar procedimientos almacenados de Employee
-- Descomentar las siguientes líneas para ejecutar la limpieza

/*
DROP PROCEDURE IF EXISTS [dbo].[100_GetEmployeeById];
DROP PROCEDURE IF EXISTS [dbo].[100_GetEmployees];
DROP PROCEDURE IF EXISTS [dbo].[100_InsertEmployee];
DROP PROCEDURE IF EXISTS [dbo].[100_UpdateEmployee];
DROP PROCEDURE IF EXISTS [dbo].[100_DeleteEmployee];
DROP PROCEDURE IF EXISTS [dbo].[100_ConvertEmployeeToUser];
DROP PROCEDURE IF EXISTS [dbo].[100_UpdateEmployeeActiveStatus];
DROP PROCEDURE IF EXISTS [dbo].[100_HasMainEmployee];

PRINT 'Procedimientos almacenados de Employee eliminados.';

-- Paso 5: Eliminar índices de Employee
DROP INDEX IF EXISTS IX_Employee_FirstName ON Employee;
DROP INDEX IF EXISTS IX_Employee_FatherLastName ON Employee;
DROP INDEX IF EXISTS IX_Employee_Email ON Employee;
DROP INDEX IF EXISTS IX_Employee_StatusId ON Employee;
DROP INDEX IF EXISTS IX_Employee_PositionId ON Employee;
DROP INDEX IF EXISTS IX_Employee_CityId ON Employee;
DROP INDEX IF EXISTS IX_Employee_RegionId ON Employee;
DROP INDEX IF EXISTS IX_Employee_UserId ON Employee;
DROP INDEX IF EXISTS IX_Employee_IsActive ON Employee;
DROP INDEX IF EXISTS IX_Employee_CreatedAt ON Employee;

PRINT 'Índices de Employee eliminados.';

-- Paso 6: Eliminar restricciones de clave foránea de Employee
-- Nota: Esto puede fallar si hay otras tablas que referencian Employee
-- En ese caso, eliminar las referencias primero

-- Paso 7: Eliminar la tabla Employee
DROP TABLE IF EXISTS Employee;

PRINT 'Tabla Employee eliminada.';

-- Paso 8: Mostrar estadísticas después de la limpieza
PRINT '=== ESTADÍSTICAS DESPUÉS DE LA LIMPIEZA ===';
SELECT 
    'Staff' AS Tabla,
    COUNT(*) AS Registros
FROM Staff
WHERE IsActive = 1;

PRINT 'Limpieza completada exitosamente.';
PRINT 'La migración de Employee a Staff está completa.';
*/

PRINT 'Limpieza cancelada. Para ejecutar la limpieza, descomente las líneas en el script.';