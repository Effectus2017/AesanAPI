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
CREATE OR ALTER FUNCTION [dbo].[GetCurrentUserId]()
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