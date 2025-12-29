-- Script maestro para corregir el error de SelectionNotification en DeliveryType
-- Este script ejecuta todas las verificaciones y correcciones necesarias
-- Ejecutar este script en la base de datos para resolver el error 500

PRINT '========================================';
PRINT 'Corrección del error SelectionNotification';
PRINT '========================================';
PRINT '';

-- Paso 1: Verificar y eliminar la columna SelectionNotification si existe
PRINT 'Paso 1: Verificando estructura de la tabla DeliveryType...';
IF EXISTS (
    SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DeliveryType' AND COLUMN_NAME = 'SelectionNotification'
)
BEGIN
    PRINT '  - La columna SelectionNotification existe. Eliminándola...';

    -- Eliminar constraint si existe
    DECLARE @ConstraintName NVARCHAR(200);
    SELECT @ConstraintName = name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('DeliveryType')
        AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('DeliveryType'), 'SelectionNotification', 'ColumnId');

    IF @ConstraintName IS NOT NULL
    BEGIN
        DECLARE @DropConstraintSQL NVARCHAR(MAX);
        SET @DropConstraintSQL = 'ALTER TABLE DeliveryType DROP CONSTRAINT ' + @ConstraintName;
        EXEC sp_executesql @DropConstraintSQL;
        PRINT '  - Constraint eliminado: ' + @ConstraintName;
    END

    -- Eliminar la columna
    ALTER TABLE DeliveryType DROP COLUMN SelectionNotification;
    PRINT '  - Columna SelectionNotification eliminada correctamente.';
END
ELSE
BEGIN
    PRINT '  - La columna SelectionNotification no existe. OK.';
END
PRINT '';

-- Paso 2: Verificar y actualizar el stored procedure 100_GetDeliveryTypesByProgram
PRINT 'Paso 2: Verificando stored procedure 100_GetDeliveryTypesByProgram...';
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[100_GetDeliveryTypesByProgram]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @ProcDefinition NVARCHAR(MAX);
    SET @ProcDefinition = OBJECT_DEFINITION(OBJECT_ID('100_GetDeliveryTypesByProgram'));

    IF @ProcDefinition LIKE '%SelectionNotification%'
    BEGIN
        PRINT '  - El stored procedure contiene referencias a SelectionNotification.';
        PRINT '  - Eliminando stored procedure antiguo...';
        DROP PROCEDURE [100_GetDeliveryTypesByProgram];
    END
    ELSE
    BEGIN
        PRINT '  - El stored procedure no contiene referencias a SelectionNotification.';
        PRINT '  - Recreando para asegurar que esté actualizado...';
        DROP PROCEDURE [100_GetDeliveryTypesByProgram];
    END
END
ELSE
BEGIN
    PRINT '  - El stored procedure no existe. Creándolo...';
END
GO

-- Crear/Actualizar el stored procedure
CREATE OR ALTER PROCEDURE [100_GetDeliveryTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dt.Id,
        dt.Name,
        dt.NameEN,
        dt.IsActive,
        dt.DisplayOrder,
        dt.CreatedAt,
        dt.UpdatedAt
    FROM DeliveryType dt
        INNER JOIN DeliveryTypeProgram dtp ON dt.Id = dtp.DeliveryTypeId
    WHERE dtp.ProgramId = @programId
        AND dt.IsActive = 1
        AND dtp.IsActive = 1
    ORDER BY dt.DisplayOrder, dt.Name;
END;
GO
PRINT '  - Stored procedure 100_GetDeliveryTypesByProgram creado/actualizado correctamente.';
PRINT '';

-- Paso 3: Verificar otros stored procedures relacionados
PRINT 'Paso 3: Verificando otros stored procedures relacionados...';

-- Verificar 100_GetAllDeliveryTypes
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[100_GetAllDeliveryTypes]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @Proc2Def NVARCHAR(MAX);
    SET @Proc2Def = OBJECT_DEFINITION(OBJECT_ID('100_GetAllDeliveryTypes'));
    IF @Proc2Def LIKE '%SelectionNotification%'
    BEGIN
        PRINT '  - ADVERTENCIA: 100_GetAllDeliveryTypes contiene referencias a SelectionNotification';
    END
    ELSE
    BEGIN
        PRINT '  - OK: 100_GetAllDeliveryTypes';
    END
END

-- Verificar 100_GetDeliveryTypeById
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[100_GetDeliveryTypeById]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @Proc3Def NVARCHAR(MAX);
    SET @Proc3Def = OBJECT_DEFINITION(OBJECT_ID('100_GetDeliveryTypeById'));
    IF @Proc3Def LIKE '%SelectionNotification%'
    BEGIN
        PRINT '  - ADVERTENCIA: 100_GetDeliveryTypeById contiene referencias a SelectionNotification';
    END
    ELSE
    BEGIN
        PRINT '  - OK: 100_GetDeliveryTypeById';
    END
END

-- Verificar 100_GetDeliveryTypesByGroupType
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[100_GetDeliveryTypesByGroupType]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @Proc4Def NVARCHAR(MAX);
    SET @Proc4Def = OBJECT_DEFINITION(OBJECT_ID('100_GetDeliveryTypesByGroupType'));
    IF @Proc4Def LIKE '%SelectionNotification%'
    BEGIN
        PRINT '  - ADVERTENCIA: 100_GetDeliveryTypesByGroupType contiene referencias a SelectionNotification';
    END
    ELSE
    BEGIN
        PRINT '  - OK: 100_GetDeliveryTypesByGroupType';
    END
END
PRINT '';

-- Paso 4: Resumen final
PRINT '========================================';
PRINT 'Corrección completada';
PRINT '========================================';
PRINT '';
PRINT 'Por favor, pruebe el endpoint nuevamente:';
PRINT 'GET /delivery-type/get-delivery-types-by-program?programId=1';
PRINT '';
GO

