-- Trigger para INSERT
CREATE OR ALTER TRIGGER [TR_Agency_Insert] ON [dbo].[Agency]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId NVARCHAR(450) = [dbo].[GetCurrentUserId]()
    
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
CREATE OR ALTER TRIGGER [TR_Agency_Update] ON [dbo].[Agency]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId NVARCHAR(450) = [dbo].[GetCurrentUserId]()
    
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
        CAST(
            CASE
                WHEN sql_variant_property(c.default_value, 'BaseType') = 'datetime2' 
                THEN FORMAT(d.[UpdatedAt], 'yyyy-MM-dd HH:mm:ss')
                ELSE d.[UpdatedAt]
            END AS NVARCHAR(MAX)
        ),
        CAST(
            CASE
                WHEN sql_variant_property(c.default_value, 'BaseType') = 'datetime2' 
                THEN FORMAT(i.[UpdatedAt], 'yyyy-MM-dd HH:mm:ss')
                ELSE i.[UpdatedAt]
            END AS NVARCHAR(MAX)
        ),
        'UPDATE',
        @UserId
    FROM inserted i
    INNER JOIN deleted d ON i.Id = d.Id
    CROSS APPLY sys.dm_exec_describe_first_result_set(N'SELECT * FROM [Agency]', NULL, 0) c
    WHERE c.is_hidden = 0
    AND (
        CASE
            WHEN sql_variant_property(c.default_value, 'BaseType') = 'datetime2' 
            THEN FORMAT(d.[UpdatedAt], 'yyyy-MM-dd HH:mm:ss')
            ELSE CAST(d.[UpdatedAt] AS NVARCHAR(MAX))
        END
        <>
        CASE
            WHEN sql_variant_property(c.default_value, 'BaseType') = 'datetime2' 
            THEN FORMAT(i.[UpdatedAt], 'yyyy-MM-dd HH:mm:ss')
            ELSE CAST(i.[UpdatedAt] AS NVARCHAR(MAX))
        END
    );
END;
GO

-- Trigger para DELETE
CREATE OR ALTER TRIGGER [TR_Agency_Delete] ON [dbo].[Agency]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId NVARCHAR(450) = [dbo].[GetCurrentUserId]()
    
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
        CAST(d.Id AS NVARCHAR(100)),
        c.name,
        CAST(
            CASE
                WHEN sql_variant_property(c.default_value, 'BaseType') = 'datetime2' 
                THEN FORMAT(d.[DeletedAt], 'yyyy-MM-dd HH:mm:ss')
                ELSE d.[DeletedAt]
            END AS NVARCHAR(MAX)
        ),
        NULL,
        'DELETE',
        @UserId
    FROM deleted d
    CROSS APPLY sys.dm_exec_describe_first_result_set(N'SELECT * FROM [Agency]', NULL, 0) c
    WHERE c.is_hidden = 0;
END;
GO 