-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: 2025-03-07 19:52:32
-- Archivo original: /Volumes/Mac/Proyectos/AESAN/Proyecto/Api/Database/StoredProcedures/Agency/100_CreateAgencyTriggers.sql
-- =============================================

-- Triggers para auditoría de la tabla Agency
-- Trigger para INSERT
CREATE OR ALTER TRIGGER [dbo].[100_TR_Agency_Insert] ON [dbo].[Agency]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId NVARCHAR(450) = [dbo].[100_GetCurrentUserId]()
    
    INSERT INTO [dbo].[AuditLog]
    (
        [TableName],
        [PrimaryKeyColumn],
        [PrimaryKeyValue],
        [ColumnName],
        [OldValue],
        [NewValue],
        [Action],
        [UserId]
    )
    SELECT 
        'Agency',
        'Id',
        CAST(i.Id AS NVARCHAR(100)),
        c.name,
        NULL,
        CAST(
            CASE
                WHEN sql_variant_property(c.default_value, 'BaseType') = 'datetime2' 
                THEN FORMAT(i.[CreatedAt], 'yyyy-MM-dd HH:mm:ss')
                ELSE i.[CreatedAt]
            END AS NVARCHAR(MAX)
        ),
        'INSERT',
        @UserId
    FROM inserted i
    CROSS APPLY sys.dm_exec_describe_first_result_set(N'SELECT * FROM [Agency]', NULL, 0) c
    WHERE c.is_hidden = 0;
END;
GO

-- Trigger para UPDATE
CREATE OR ALTER TRIGGER [dbo].[100_TR_Agency_Update] ON [dbo].[Agency]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId NVARCHAR(450) = [dbo].[100_GetCurrentUserId]()
    
    INSERT INTO [dbo].[AuditLog]
    (
        [TableName],
        [PrimaryKeyColumn],
        [PrimaryKeyValue],
        [ColumnName],
        [OldValue],
        [NewValue],
        [Action],
        [UserId]
    )
    SELECT 
        'Agency',
        'Id',
        CAST(i.Id AS NVARCHAR(100)),
        c.name,
        CAST(d.[UpdatedAt] AS NVARCHAR(MAX)),
        CAST(i.[UpdatedAt] AS NVARCHAR(MAX)),
        'UPDATE',
        @UserId
    FROM inserted i
    INNER JOIN deleted d ON i.Id = d.Id
    CROSS APPLY sys.dm_exec_describe_first_result_set(N'SELECT * FROM [Agency]', NULL, 0) c
    WHERE c.is_hidden = 0
    AND ISNULL(d.[UpdatedAt], '') <> ISNULL(i.[UpdatedAt], '');
END;
GO

-- Trigger para DELETE (Soft Delete)
CREATE OR ALTER TRIGGER [dbo].[100_TR_Agency_Delete] ON [dbo].[Agency]
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
        DECLARE @UserId NVARCHAR(450) = [dbo].[100_GetCurrentUserId]()
        
        INSERT INTO [dbo].[AuditLog]
        (
            [TableName],
            [PrimaryKeyColumn],
            [PrimaryKeyValue],
            [ColumnName],
            [OldValue],
            [NewValue],
            [Action],
            [UserId]
        )
        SELECT 
            'Agency',
            'Id',
            CAST(i.Id AS NVARCHAR(100)),
            'IsActive',
            '1',
            '0',
            'DELETE',
            @UserId
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsActive = 0 AND d.IsActive = 1;
    END
END;
GO 
GO
