-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: 2025-03-07 19:52:14
-- Archivo original: /Volumes/Mac/Proyectos/AESAN/Proyecto/Api/Database/StoredProcedures/Audit/100_AllStoredProceduresForAudit.sql
-- =============================================

-- Archivo consolidado de procedimientos almacenados para Auditoría
-- Versión: 1.0.0
-- Fecha: 2024-03-19

-- Tabla de Auditoría
CREATE TABLE [dbo].[AuditLog] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TableName] NVARCHAR(100) NOT NULL,
    [PrimaryKeyColumn] NVARCHAR(100) NOT NULL,
    [PrimaryKeyValue] NVARCHAR(100) NOT NULL,
    [ColumnName] NVARCHAR(100) NOT NULL,
    [OldValue] NVARCHAR(MAX),
    [NewValue] NVARCHAR(MAX),
    [Action] NVARCHAR(10) NOT NULL, -- INSERT, UPDATE, DELETE
    [UserId] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1
);
GO

-- Función para obtener el UserId actual
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

-- Procedimiento para obtener el historial de auditoría
CREATE OR ALTER PROCEDURE [dbo].[100_GetAuditLogHistory]
    @TableName NVARCHAR(100) = NULL,
    @PrimaryKeyValue NVARCHAR(100) = NULL,
    @FromDate DATETIME2 = NULL,
    @ToDate DATETIME2 = NULL,
    @UserId NVARCHAR(450) = NULL,
    @Action NVARCHAR(10) = NULL,
    @Take INT = 10,
    @Skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM [dbo].[AuditLog]
    WHERE (@TableName IS NULL OR TableName = @TableName)
    AND (@PrimaryKeyValue IS NULL OR PrimaryKeyValue = @PrimaryKeyValue)
    AND (@FromDate IS NULL OR CreatedAt >= @FromDate)
    AND (@ToDate IS NULL OR CreatedAt <= @ToDate)
    AND (@UserId IS NULL OR UserId = @UserId)
    AND (@Action IS NULL OR Action = @Action)
    AND IsActive = 1
    ORDER BY CreatedAt DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

    SELECT COUNT(*)
    FROM [dbo].[AuditLog]
    WHERE (@TableName IS NULL OR TableName = @TableName)
    AND (@PrimaryKeyValue IS NULL OR PrimaryKeyValue = @PrimaryKeyValue)
    AND (@FromDate IS NULL OR CreatedAt >= @FromDate)
    AND (@ToDate IS NULL OR CreatedAt <= @ToDate)
    AND (@UserId IS NULL OR UserId = @UserId)
    AND (@Action IS NULL OR Action = @Action)
    AND IsActive = 1;
END;
GO 
GO
