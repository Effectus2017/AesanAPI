-- Versión: 1.0.0
-- Descripción: Creación de tablas y objetos de auditoría
-- Fecha: 2024-03-19

-- Tabla de Auditoría
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLog]') AND type in (N'U'))
BEGIN
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
END;
GO

-- Función para obtener el UserId actual
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[100_GetCurrentUserId]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION [dbo].[100_GetCurrentUserId];
GO

CREATE FUNCTION [dbo].[100_GetCurrentUserId]()
RETURNS NVARCHAR(450)
AS
BEGIN
    DECLARE @UserId NVARCHAR(450);
    SELECT @UserId = CAST(SESSION_CONTEXT(N'UserId') AS NVARCHAR(450));
    IF @UserId IS NULL
        SET @UserId = 'SYSTEM';
    RETURN @UserId;
END;
GO

-- Procedimiento para obtener el historial de auditoría
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[100_GetAuditLogHistory]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[100_GetAuditLogHistory];
GO

CREATE PROCEDURE [dbo].[100_GetAuditLogHistory]
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

-- Trigger para INSERT
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[100_TR_Agency_Insert]'))
    DROP TRIGGER [dbo].[100_TR_Agency_Insert];
GO

CREATE TRIGGER [dbo].[100_TR_Agency_Insert] ON [dbo].[Agency]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId NVARCHAR(450) = [dbo].[100_GetCurrentUserId]();
    
    INSERT INTO [dbo].[AuditLog]
    ([TableName], [PrimaryKeyColumn], [PrimaryKeyValue], [ColumnName], [OldValue], [NewValue], [Action], [UserId])
    SELECT 
        'Agency', 'Id', CAST(i.Id AS NVARCHAR(100)), c.name,
        NULL,
        CASE
            WHEN c.system_type_name LIKE '%datetime%' 
            THEN FORMAT(i.[CreatedAt], 'yyyy-MM-dd HH:mm:ss')
            ELSE CAST(i.[CreatedAt] AS NVARCHAR(MAX))
        END,
        'INSERT',
        @UserId
    FROM inserted i
    CROSS APPLY sys.dm_exec_describe_first_result_set(N'SELECT * FROM [dbo].[Agency]', NULL, 0) c
    WHERE c.is_hidden = 0;
END;
GO

-- Trigger para UPDATE
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[100_TR_Agency_Update]'))
    DROP TRIGGER [dbo].[100_TR_Agency_Update];
GO

CREATE TRIGGER [dbo].[100_TR_Agency_Update] ON [dbo].[Agency]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId NVARCHAR(450) = [dbo].[100_GetCurrentUserId]();
    
    INSERT INTO [dbo].[AuditLog]
    ([TableName], [PrimaryKeyColumn], [PrimaryKeyValue], [ColumnName], [OldValue], [NewValue], [Action], [UserId])
    SELECT 
        'Agency', 'Id', CAST(i.Id AS NVARCHAR(100)), c.name,
        CAST(d.[UpdatedAt] AS NVARCHAR(MAX)),
        CAST(i.[UpdatedAt] AS NVARCHAR(MAX)),
        'UPDATE',
        @UserId
    FROM inserted i
    INNER JOIN deleted d ON i.Id = d.Id
    CROSS APPLY sys.dm_exec_describe_first_result_set(N'SELECT * FROM [dbo].[Agency]', NULL, 0) c
    WHERE c.is_hidden = 0
    AND ISNULL(d.[UpdatedAt], '') <> ISNULL(i.[UpdatedAt], '');
END;
GO

-- Trigger para DELETE (soft delete)
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[100_TR_Agency_Delete]'))
    DROP TRIGGER [dbo].[100_TR_Agency_Delete];
GO

CREATE TRIGGER [dbo].[100_TR_Agency_Delete] ON [dbo].[Agency]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1 
        FROM inserted i 
        INNER JOIN deleted d ON i.Id = d.Id 
        WHERE i.IsActive = 0 AND d.IsActive = 1
    )
    BEGIN
        DECLARE @UserId NVARCHAR(450) = [dbo].[100_GetCurrentUserId]();
        
        INSERT INTO [dbo].[AuditLog]
        ([TableName], [PrimaryKeyColumn], [PrimaryKeyValue], [ColumnName], [OldValue], [NewValue], [Action], [UserId])
        SELECT 
            'Agency', 'Id', CAST(i.Id AS NVARCHAR(100)), 'IsActive',
            '1', '0', 'DELETE', @UserId
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsActive = 0 AND d.IsActive = 1;
    END
END;
GO 