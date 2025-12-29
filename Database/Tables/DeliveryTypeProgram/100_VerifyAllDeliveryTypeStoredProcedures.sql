-- Script de verificación de todos los stored procedures relacionados con DeliveryType
-- Verifica que no contengan referencias a SelectionNotification

PRINT '=== Verificación de Stored Procedures de DeliveryType ===';
PRINT '';

-- Verificar 100_GetDeliveryTypesByProgram
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[100_GetDeliveryTypesByProgram]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @Proc1Definition NVARCHAR(MAX);
    SET @Proc1Definition = OBJECT_DEFINITION(OBJECT_ID('100_GetDeliveryTypesByProgram'));
    
    IF @Proc1Definition LIKE '%SelectionNotification%'
    BEGIN
        PRINT 'ERROR: 100_GetDeliveryTypesByProgram contiene referencias a SelectionNotification';
    END
    ELSE
    BEGIN
        PRINT 'OK: 100_GetDeliveryTypesByProgram no contiene referencias a SelectionNotification';
    END
END
ELSE
BEGIN
    PRINT 'ADVERTENCIA: 100_GetDeliveryTypesByProgram no existe';
END

-- Verificar 100_GetAllDeliveryTypes
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[100_GetAllDeliveryTypes]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @Proc2Definition NVARCHAR(MAX);
    SET @Proc2Definition = OBJECT_DEFINITION(OBJECT_ID('100_GetAllDeliveryTypes'));
    
    IF @Proc2Definition LIKE '%SelectionNotification%'
    BEGIN
        PRINT 'ERROR: 100_GetAllDeliveryTypes contiene referencias a SelectionNotification';
    END
    ELSE
    BEGIN
        PRINT 'OK: 100_GetAllDeliveryTypes no contiene referencias a SelectionNotification';
    END
END
ELSE
BEGIN
    PRINT 'ADVERTENCIA: 100_GetAllDeliveryTypes no existe';
END

-- Verificar 100_GetDeliveryTypeById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[100_GetDeliveryTypeById]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @Proc3Definition NVARCHAR(MAX);
    SET @Proc3Definition = OBJECT_DEFINITION(OBJECT_ID('100_GetDeliveryTypeById'));
    
    IF @Proc3Definition LIKE '%SelectionNotification%'
    BEGIN
        PRINT 'ERROR: 100_GetDeliveryTypeById contiene referencias a SelectionNotification';
    END
    ELSE
    BEGIN
        PRINT 'OK: 100_GetDeliveryTypeById no contiene referencias a SelectionNotification';
    END
END
ELSE
BEGIN
    PRINT 'ADVERTENCIA: 100_GetDeliveryTypeById no existe';
END

-- Verificar 100_GetDeliveryTypesByGroupType
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[100_GetDeliveryTypesByGroupType]') AND type in (N'P', N'PC'))
BEGIN
    DECLARE @Proc4Definition NVARCHAR(MAX);
    SET @Proc4Definition = OBJECT_DEFINITION(OBJECT_ID('100_GetDeliveryTypesByGroupType'));
    
    IF @Proc4Definition LIKE '%SelectionNotification%'
    BEGIN
        PRINT 'ERROR: 100_GetDeliveryTypesByGroupType contiene referencias a SelectionNotification';
    END
    ELSE
    BEGIN
        PRINT 'OK: 100_GetDeliveryTypesByGroupType no contiene referencias a SelectionNotification';
    END
END
ELSE
BEGIN
    PRINT 'ADVERTENCIA: 100_GetDeliveryTypesByGroupType no existe';
END

PRINT '';
PRINT '=== Verificación completada ===';
GO

