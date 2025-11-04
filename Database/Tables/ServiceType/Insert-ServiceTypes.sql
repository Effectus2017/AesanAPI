-- =============================================
-- Script: Insert Service Types
-- Descripción: Inserta los tipos de servicio base en ServiceType
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

-- IDs fijos predefinidos (1-10 para servicios base)
-- Estos IDs NUNCA deben cambiar

-- Verificar si la columna Id tiene IDENTITY antes de usar SET IDENTITY_INSERT
DECLARE @hasIdentity BIT = 0;

IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[ServiceType]') AND type in (N'U'))
BEGIN
    SELECT @hasIdentity = CASE 
        WHEN is_identity = 1 THEN 1 
        ELSE 0 
    END
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[ServiceType]')
        AND name = 'Id';

    IF @hasIdentity = 0
    BEGIN
        RAISERROR('La columna Id no tiene IDENTITY. Ejecutar Update-ServiceType-AddIdentity.sql primero.', 16, 1);
        RETURN;
    END
END
ELSE
BEGIN
    RAISERROR('La tabla ServiceType no existe. Ejecutar ServiceType-Table.sql primero.', 16, 1);
    RETURN;
END

SET IDENTITY_INSERT [dbo].[ServiceType] ON;

-- Verificar si ya existen registros
IF NOT EXISTS (SELECT 1
FROM [dbo].[ServiceType]
WHERE Id BETWEEN 1 AND 10)
BEGIN
    INSERT INTO [dbo].[ServiceType]
        ([Id], [Name], [NameEN], [Code], [Description], [DisplayOrder], [IsActive], [CreatedAt])
    VALUES
        (1, 'Desayuno', 'Breakfast', 'BREAKFAST', 'Servicio de desayuno', 1, 1, GETDATE()),
        (2, 'Almuerzo', 'Lunch', 'LUNCH', 'Servicio de almuerzo', 2, 1, GETDATE()),
        (3, 'Merienda Matutina', 'Morning Snack', 'SNACK_AM', 'Servicio de merienda matutina', 3, 1, GETDATE()),
        (4, 'Cena', 'Dinner', 'DINNER', 'Servicio de cena', 4, 1, GETDATE()),
        (5, 'Merienda Vespertina', 'Afternoon Snack', 'SNACK_PM', 'Servicio de merienda vespertina', 5, 1, GETDATE()),
        (6, 'Merienda Nocturna', 'Night Snack', 'SNACK_NIGHT', 'Servicio de merienda nocturna', 6, 1, GETDATE()),
        (7, 'Cena Extendida', 'Extended Dinner', 'DINNER_EXTENDED', 'Servicio de cena extendida', 7, 1, GETDATE()),
        (8, 'Cena en Riesgo', 'Dinner at Risk', 'DINNER_AT_RISK', 'Servicio de cena en riesgo', 8, 1, GETDATE()),
        (9, 'Merienda Extendida', 'Extended Snack', 'SNACK_EXTENDED', 'Servicio de merienda extendida', 9, 1, GETDATE()),
        (10, 'Merienda en Riesgo', 'Snack at Risk', 'SNACK_AT_RISK', 'Servicio de merienda en riesgo', 10, 1, GETDATE());

    PRINT 'Tipos de servicio insertados exitosamente';

    -- Resetear IDENTITY para que el siguiente valor sea 11 (o superior si ya hay más registros)
    -- Esto asegura que futuros servicios (ID >= 11) se inserten automáticamente
    DECLARE @maxId INT;
    SELECT @maxId = ISNULL(MAX(Id), 10)
    FROM [dbo].[ServiceType];
    DBCC CHECKIDENT('[dbo].[ServiceType]', RESEED, @maxId);
    PRINT 'IDENTITY reseed a ' + CAST(@maxId AS VARCHAR);
END
ELSE
BEGIN
    PRINT 'Los tipos de servicio ya existen';
END

SET IDENTITY_INSERT [dbo].[ServiceType] OFF;
GO

-- Verificar que se insertaron correctamente
SELECT
    Id,
    Code,
    Name,
    NameEN,
    DisplayOrder,
    IsActive
FROM [dbo].[ServiceType]
ORDER BY DisplayOrder;
GO

