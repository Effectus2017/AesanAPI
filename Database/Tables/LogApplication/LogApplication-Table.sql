-- =============================================
-- Tabla: LogApplication (errores de aplicación - sistema central de logging)
-- =============================================
-- Esquema unificado: mismas columnas que LogAudit, LogEmail, LogJob.
-- Sustituye a ELMAH_Error. Categoría Application.
-- =============================================

CREATE TABLE [dbo].[LogApplication]
(
    [Id] BIGINT PRIMARY KEY IDENTITY(1,1),
    [Category] NVARCHAR(50) NOT NULL DEFAULT 'Application',
    [Level] NVARCHAR(20) NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [Payload] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UserId] NVARCHAR(450) NULL,
    [Status] NVARCHAR(50) NULL
);

CREATE NONCLUSTERED INDEX [IX_LogApplication_CreatedAt] ON [dbo].[LogApplication]([CreatedAt] DESC);
CREATE NONCLUSTERED INDEX [IX_LogApplication_Level] ON [dbo].[LogApplication]([Level]);
CREATE NONCLUSTERED INDEX [IX_LogApplication_UserId] ON [dbo].[LogApplication]([UserId]);
GO
