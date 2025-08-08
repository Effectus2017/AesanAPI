-- =============================================
-- Tabla: Messages
-- Descripción: Sistema de mensajes para la aplicación
-- =============================================

CREATE TABLE [dbo].[Messages]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Icon] [nvarchar](255) NULL,
    [Image] [nvarchar](500) NULL,
    [Title] [nvarchar](255) NOT NULL,
    [Description] [nvarchar](1000) NULL,
    [Time] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [Link] [nvarchar](500) NULL,
    [UseRouter] [bit] NOT NULL DEFAULT 0,
    [Read] [bit] NOT NULL DEFAULT 0,
    [UserId] [nvarchar](450) NULL,
    -- Para asociar mensajes a usuarios específicos
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Índices para optimizar consultas
CREATE NONCLUSTERED INDEX [IX_Messages_UserId] ON [dbo].[Messages] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_Messages_Read] ON [dbo].[Messages] ([Read]);
CREATE NONCLUSTERED INDEX [IX_Messages_Time] ON [dbo].[Messages] ([Time] DESC);
CREATE NONCLUSTERED INDEX [IX_Messages_IsDeleted] ON [dbo].[Messages] ([IsDeleted]);