-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: 2025-03-07 19:52:36
-- Archivo original: /Volumes/Mac/Proyectos/AESAN/Proyecto/Api/Database/StoredProcedures/Audit/100_GetCurrentUserId.sql
-- =============================================

-- Función para obtener el UserId actual del contexto
CREATE OR ALTER FUNCTION [dbo].[100_GetCurrentUserId]()
RETURNS NVARCHAR(450)
AS
BEGIN
    DECLARE @UserId NVARCHAR(450)
    
    -- Intentar obtener el ID del contexto de la sesión
    SELECT @UserId = CAST(SESSION_CONTEXT(N'UserId') AS NVARCHAR(450))
    
    -- Si es NULL, usar un valor por defecto
    IF @UserId IS NULL
        SET @UserId = 'SYSTEM'
        
    RETURN @UserId
END;
GO 
GO
