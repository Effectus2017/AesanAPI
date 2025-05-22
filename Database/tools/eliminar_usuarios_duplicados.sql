-- Script para identificar y eliminar usuarios duplicados en AspNetIdentity
-- Autor: Equipo de Desarrollo
-- Fecha: 2025-03-14

-- PARTE 1: IDENTIFICACIÓN DE USUARIOS DUPLICADOS
-- =============================================

-- Identificar usuarios duplicados por Email
WITH DuplicateEmails AS (
    SELECT 
        Email,
        COUNT(*) AS EmailCount,
        STRING_AGG(Id, ', ') AS UserIds
    FROM AspNetUsers
    WHERE Email IS NOT NULL
    GROUP BY Email
    HAVING COUNT(*) > 1
)
SELECT 
    'Usuarios con Email duplicado' AS TipoDuplicado,
    Email,
    EmailCount AS Cantidad,
    UserIds AS IdsUsuarios
FROM DuplicateEmails
ORDER BY EmailCount DESC;

-- Identificar usuarios duplicados por UserName
WITH DuplicateUserNames AS (
    SELECT 
        UserName,
        COUNT(*) AS UserNameCount,
        STRING_AGG(Id, ', ') AS UserIds
    FROM AspNetUsers
    WHERE UserName IS NOT NULL
    GROUP BY UserName
    HAVING COUNT(*) > 1
)
SELECT 
    'Usuarios con UserName duplicado' AS TipoDuplicado,
    UserName,
    UserNameCount AS Cantidad,
    UserIds AS IdsUsuarios
FROM DuplicateUserNames
ORDER BY UserNameCount DESC;

-- Identificar usuarios duplicados por combinación de nombre y apellidos
WITH DuplicateNames AS (
    SELECT 
        FirstName + ' ' + ISNULL(MiddleName + ' ', '') + FatherLastName + ' ' + MotherLastName AS FullName,
        COUNT(*) AS NameCount,
        STRING_AGG(Id, ', ') AS UserIds
    FROM AspNetUsers
    GROUP BY FirstName + ' ' + ISNULL(MiddleName + ' ', '') + FatherLastName + ' ' + MotherLastName
    HAVING COUNT(*) > 1
)
SELECT 
    'Usuarios con nombre completo duplicado' AS TipoDuplicado,
    FullName,
    NameCount AS Cantidad,
    UserIds AS IdsUsuarios
FROM DuplicateNames
ORDER BY NameCount DESC;

-- PARTE 2: ANÁLISIS DETALLADO DE DUPLICADOS
-- =========================================

-- Obtener información detallada de los usuarios con email duplicado
WITH DuplicateEmails AS (
    SELECT 
        Email,
        COUNT(*) AS EmailCount
    FROM AspNetUsers
    WHERE Email IS NOT NULL
    GROUP BY Email
    HAVING COUNT(*) > 1
)
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.MiddleName,
    u.FatherLastName,
    u.MotherLastName,
    u.EmailConfirmed,
    u.PhoneNumber,
    u.IsActive,
    u.CreatedAt,
    u.UpdatedAt,
    u.AgencyId,
    (SELECT COUNT(*) FROM AspNetUserRoles WHERE UserId = u.Id) AS RolesCount,
    (SELECT STRING_AGG(r.Name, ', ') FROM AspNetRoles r 
     INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId 
     WHERE ur.UserId = u.Id) AS Roles
FROM AspNetUsers u
INNER JOIN DuplicateEmails d ON u.Email = d.Email
ORDER BY u.Email, u.CreatedAt;

-- PARTE 3: SCRIPT PARA ELIMINAR DUPLICADOS (GENERAR AUTOMÁTICAMENTE)
-- =================================================================

-- Este script generará las instrucciones DELETE para eliminar los usuarios duplicados
-- manteniendo el registro más reciente o el que tenga más roles asignados

PRINT '-- IMPORTANTE: Revisa cuidadosamente este script antes de ejecutarlo';
PRINT '-- Se recomienda hacer un respaldo de la base de datos antes de ejecutar estas instrucciones';
PRINT '-- Las siguientes instrucciones eliminarán los usuarios duplicados, manteniendo el más reciente';
PRINT '';

-- Generar script para eliminar duplicados por Email
WITH DuplicateEmails AS (
    SELECT 
        Email,
        COUNT(*) AS EmailCount
    FROM AspNetUsers
    WHERE Email IS NOT NULL
    GROUP BY Email
    HAVING COUNT(*) > 1
),
RankedUsers AS (
    SELECT 
        u.Id,
        u.Email,
        ROW_NUMBER() OVER (PARTITION BY u.Email ORDER BY 
            u.IsActive DESC, -- Primero los activos
            (SELECT COUNT(*) FROM AspNetUserRoles WHERE UserId = u.Id) DESC, -- Luego los que tienen más roles
            u.CreatedAt DESC -- Finalmente los más recientes
        ) AS RowNum
    FROM AspNetUsers u
    INNER JOIN DuplicateEmails d ON u.Email = d.Email
)
SELECT 
    '-- Eliminando duplicados para email: ' + Email + CHAR(13) + CHAR(10) +
    '-- Manteniendo el usuario con ID: ' + Id + CHAR(13) + CHAR(10) +
    'BEGIN TRANSACTION;' + CHAR(13) + CHAR(10) +
    'DELETE FROM AspNetUserClaims WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = ''' + Email + ''' AND Id <> ''' + Id + ''');' + CHAR(13) + CHAR(10) +
    'DELETE FROM AspNetUserLogins WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = ''' + Email + ''' AND Id <> ''' + Id + ''');' + CHAR(13) + CHAR(10) +
    'DELETE FROM AspNetUserRoles WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = ''' + Email + ''' AND Id <> ''' + Id + ''');' + CHAR(13) + CHAR(10) +
    'DELETE FROM AspNetUsers WHERE Email = ''' + Email + ''' AND Id <> ''' + Id + ''';' + CHAR(13) + CHAR(10) +
    'COMMIT;' + CHAR(13) + CHAR(10)
FROM RankedUsers
WHERE RowNum = 1;

-- PARTE 4: INSTRUCCIONES PARA PREVENIR DUPLICADOS EN EL FUTURO
-- ===========================================================

PRINT '-- Para prevenir duplicados en el futuro, considera agregar estos índices únicos:';
PRINT '';
PRINT 'IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = ''IX_AspNetUsers_Email'' AND object_id = OBJECT_ID(''AspNetUsers''))';
PRINT 'CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_Email ON AspNetUsers(Email) WHERE Email IS NOT NULL;';
PRINT '';
PRINT 'IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = ''IX_AspNetUsers_UserName'' AND object_id = OBJECT_ID(''AspNetUsers''))';
PRINT 'CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_UserName ON AspNetUsers(UserName) WHERE UserName IS NOT NULL;'; 