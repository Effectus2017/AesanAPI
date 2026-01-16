-- Script de verificación para ELMAH
-- Este script verifica que las tablas y stored procedures de ELMAH existan

-- Verificar si la tabla existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_Error]') AND type in (N'U'))
BEGIN
    PRINT 'ERROR: La tabla ELMAH_Error NO existe. Ejecute el script ELMAH_Error.sql primero.'
END
ELSE
BEGIN
    PRINT 'OK: La tabla ELMAH_Error existe.'
    
    -- Verificar columnas
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_Error]') AND name = 'ErrorId')
        PRINT 'OK: Columna ErrorId existe.'
    ELSE
        PRINT 'ERROR: Columna ErrorId NO existe.'
        
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_Error]') AND name = 'AllXml')
        PRINT 'OK: Columna AllXml existe.'
    ELSE
        PRINT 'ERROR: Columna AllXml NO existe.'
END

-- Verificar stored procedures
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_LogError]') AND type in (N'P', N'PC'))
BEGIN
    PRINT 'ERROR: El stored procedure ELMAH_LogError NO existe. Ejecute el script ELMAH_Error.sql primero.'
END
ELSE
BEGIN
    PRINT 'OK: El stored procedure ELMAH_LogError existe.'
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_GetErrorXml]') AND type in (N'P', N'PC'))
BEGIN
    PRINT 'ERROR: El stored procedure ELMAH_GetErrorXml NO existe. Ejecute el script ELMAH_Error.sql primero.'
END
ELSE
BEGIN
    PRINT 'OK: El stored procedure ELMAH_GetErrorXml existe.'
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_GetErrorsXml]') AND type in (N'P', N'PC'))
BEGIN
    PRINT 'ERROR: El stored procedure ELMAH_GetErrorsXml NO existe. Ejecute el script ELMAH_Error.sql primero.'
END
ELSE
BEGIN
    PRINT 'OK: El stored procedure ELMAH_GetErrorsXml existe.'
END

-- Verificar permisos (solo información)
PRINT ''
PRINT 'Verificando permisos...'
PRINT 'Usuario actual: ' + SYSTEM_USER
PRINT 'Verifique que el usuario tenga permisos INSERT, SELECT en la tabla ELMAH_Error'

-- Contar errores existentes
DECLARE @ErrorCount INT
SELECT @ErrorCount = COUNT(*) FROM [dbo].[ELMAH_Error]
PRINT ''
PRINT 'Total de errores registrados en ELMAH: ' + CAST(@ErrorCount AS VARCHAR(10))
