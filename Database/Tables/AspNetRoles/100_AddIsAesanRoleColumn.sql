-- Migración: Añadir columna IsAesanRole a AspNetRoles
-- Roles con IsAesanRole=1 participan en la selección multi-rol tras login

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('AspNetRoles') AND name = 'IsAesanRole'
)
BEGIN
    ALTER TABLE AspNetRoles
    ADD IsAesanRole BIT NOT NULL DEFAULT 0;

    -- Marcar roles AESAN existentes
    UPDATE AspNetRoles
    SET IsAesanRole = 1
    WHERE Name IN ('Administrator', 'Monitor', 'SuperAdmin', 'Program-Coordinator');

    PRINT 'Columna IsAesanRole añadida a AspNetRoles.';
END
ELSE
BEGIN
    PRINT 'Columna IsAesanRole ya existe en AspNetRoles.';
END
