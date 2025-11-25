/*
===========================================
Script de Migración: Renombrar Columnas de Description
===========================================
Fecha: 2025-01-XX
Versión: 1.1

Propósito:
- Renombrar DescriptionES/DescriptionEN a BodyES/BodyEN (contenido del mensaje)
- Renombrar Description a PurposeES (propósito del template en español)
- Renombrar TemplateDescriptionEN a PurposeEN (propósito del template en inglés)

Esto elimina la confusión de tener múltiples campos con "description" en el nombre.
*/

-- Verificar y renombrar DescriptionES a BodyES
IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'DescriptionES'
)
BEGIN
    EXEC sp_rename '[dbo].[MessageTemplate].[DescriptionES]', 'BodyES', 'COLUMN';
    PRINT 'Columna DescriptionES renombrada exitosamente a BodyES';
END
ELSE IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'BodyES'
)
BEGIN
    PRINT 'La columna BodyES ya existe en la tabla MessageTemplate';
END
GO

-- Verificar y renombrar DescriptionEN a BodyEN
IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'DescriptionEN'
)
BEGIN
    EXEC sp_rename '[dbo].[MessageTemplate].[DescriptionEN]', 'BodyEN', 'COLUMN';
    PRINT 'Columna DescriptionEN renombrada exitosamente a BodyEN';
END
ELSE IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'BodyEN'
)
BEGIN
    PRINT 'La columna BodyEN ya existe en la tabla MessageTemplate';
END
GO

-- Verificar y renombrar Description a PurposeES
IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'Description'
)
BEGIN
    EXEC sp_rename '[dbo].[MessageTemplate].[Description]', 'PurposeES', 'COLUMN';
    PRINT 'Columna Description renombrada exitosamente a PurposeES';
END
ELSE IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'PurposeES'
)
BEGIN
    PRINT 'La columna PurposeES ya existe en la tabla MessageTemplate';
END
GO

-- Verificar y renombrar TemplateDescriptionEN a PurposeEN
IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'TemplateDescriptionEN'
)
BEGIN
    EXEC sp_rename '[dbo].[MessageTemplate].[TemplateDescriptionEN]', 'PurposeEN', 'COLUMN';
    PRINT 'Columna TemplateDescriptionEN renombrada exitosamente a PurposeEN';
END
ELSE IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[MessageTemplate]')
    AND name = 'PurposeEN'
)
BEGIN
    PRINT 'La columna PurposeEN ya existe en la tabla MessageTemplate';
END
GO

