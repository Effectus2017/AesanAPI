-- =============================================
-- Tabla: Messages
-- Descripción: Sistema de mensajes para la aplicación
-- =============================================

CREATE TABLE Messages
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Icon NVARCHAR(255) NULL,
    Image NVARCHAR(500) NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Time DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    Link NVARCHAR(500) NULL,
    UseRouter BIT NOT NULL DEFAULT 0,
    [Read] BIT NOT NULL DEFAULT 0,
    UserId NVARCHAR(450) NULL,
    -- Para asociar mensajes a usuarios específicos
    CreatedAt DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
);

-- Índices para optimizar consultas
CREATE NONCLUSTERED INDEX [IX_Messages_UserId] ON [dbo].[Messages] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_Messages_Read] ON [dbo].[Messages] ([Read]);
CREATE NONCLUSTERED INDEX [IX_Messages_Time] ON [dbo].[Messages] ([Time] DESC);
CREATE NONCLUSTERED INDEX [IX_Messages_IsDeleted] ON [dbo].[Messages] ([IsDeleted]);

INSERT INTO Messages
    (Title, Description, Time, Link, UseRouter, [Read], UserId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
    ('Test', 'Test', GETDATE(), 'https://www.google.com', 0, 0, '1', GETDATE(), GETDATE(), 0);